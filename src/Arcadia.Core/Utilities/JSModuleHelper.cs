using Microsoft.JSInterop;

namespace Arcadia.Core.Utilities;

/// <summary>
/// Shared helpers for loading and disposing JavaScript modules from Razor components.
/// Centralises the exception-handling pattern (JSDisconnectedException on Server
/// circuit teardown, ObjectDisposedException on double-dispose, InvalidOperationException
/// during SSR prerendering) so individual components don't need to duplicate it.
///
/// <para>Use this when a component manages its own <see cref="IJSObjectReference"/>
/// lifetime — for example because it loads multiple modules, or loads them lazily
/// on demand rather than eagerly in <c>OnAfterRenderAsync</c>. Components that
/// load exactly one module up front should inherit from
/// <see cref="Base.ArcadiaInteropBase"/> instead.</para>
/// </summary>
public static class JSModuleHelper
{
    /// <summary>
    /// Imports a JavaScript ES module by path, swallowing the three exceptions that are
    /// normal during teardown and SSR prerender. Returns <c>null</c> if the import
    /// could not happen — callers should treat null as "JS interop unavailable" and
    /// skip the interop call, not retry.
    /// </summary>
    /// <param name="js">The injected <see cref="IJSRuntime"/>.</param>
    /// <param name="modulePath">e.g. <c>./_content/Arcadia.Charts/js/chart-interop.js</c>.</param>
    public static async ValueTask<IJSObjectReference?> ImportSafelyAsync(IJSRuntime js, string modulePath)
    {
        try
        {
            return await js.InvokeAsync<IJSObjectReference>("import", modulePath);
        }
        catch (JSException) { return null; }
        catch (InvalidOperationException) { return null; }
#if NET6_0_OR_GREATER
        catch (JSDisconnectedException) { return null; }
#endif
    }

    /// <summary>
    /// Disposes a JS object reference, swallowing the two exceptions that fire when
    /// the underlying circuit is already gone (JSDisconnectedException on net6.0+) or
    /// the reference has already been disposed (<see cref="ObjectDisposedException"/>).
    /// Safe to call with a <c>null</c> reference.
    /// </summary>
    public static async ValueTask DisposeSafelyAsync(IJSObjectReference? module)
    {
        if (module is null) return;
        try
        {
            await module.DisposeAsync();
        }
#if NET6_0_OR_GREATER
        catch (JSDisconnectedException) { /* circuit already torn down */ }
#endif
        catch (ObjectDisposedException) { /* already disposed */ }
    }
}
