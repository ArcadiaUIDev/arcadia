using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Arcadia.Core.Accessibility;

/// <summary>
/// Traps keyboard focus within its child content. Used for modals, dialogs,
/// and other overlay components that require focus containment for accessibility.
/// </summary>
public partial class FocusTrap : Base.ArcadiaComponentBase, IAsyncDisposable
{
    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    /// <summary>
    /// Gets or sets the child content to render inside the focus trap.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets whether the focus trap is currently active.
    /// When true, focus is constrained within the trap boundaries.
    /// </summary>
    [Parameter]
    public bool Active { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to automatically focus the first focusable element when activated.
    /// </summary>
    [Parameter]
    public bool AutoFocus { get; set; } = true;

    private ElementReference _trapElement;
    private IJSObjectReference? _module;
    private bool _disposed;

    private string? CssClass => Utilities.CssBuilder.Default("arcadia-focus-trap")
        .AddClass("arcadia-focus-trap--active", Active)
        .AddClass(Class)
        .Build();

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && Active && AutoFocus)
        {
            await FocusFirstElementAsync();
        }
    }

    private async Task OnSentinelFocusAsync()
    {
        await FocusFirstElementAsync();
    }

    private async Task FocusFirstElementAsync()
    {
        if (_disposed) return;
        _module ??= await Utilities.JSModuleHelper.ImportSafelyAsync(JSRuntime, "./_content/Arcadia.Core/js/focusTrap.js");
        if (_disposed || _module is null) return; // disposed during await, or interop unavailable
        try
        {
            await _module.InvokeVoidAsync("focusFirst", _trapElement);
        }
#if NET6_0_OR_GREATER
        catch (JSDisconnectedException) { /* Circuit disconnected — safe to ignore */ }
#endif
        catch (Exception) { /* net5.0 fallback */ }
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        _disposed = true;
        await Utilities.JSModuleHelper.DisposeSafelyAsync(_module);
        GC.SuppressFinalize(this);
    }
}
