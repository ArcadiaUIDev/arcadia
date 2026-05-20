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
        try
        {
            _module ??= await JSRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./_content/Arcadia.Core/js/focusTrap.js");
            if (_disposed) return; // disposed during await
            await _module.InvokeVoidAsync("focusFirst", _trapElement);
        }
#if NET6_0_OR_GREATER
        catch (JSDisconnectedException)
        {
            // Circuit disconnected — safe to ignore
        }
#endif
        catch (Exception)
        {
            // Fallback for net5.0 where JSDisconnectedException doesn't exist
        }
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        _disposed = true;
        if (_module is not null)
        {
            try
            {
                await _module.DisposeAsync();
            }
#if NET6_0_OR_GREATER
            catch (JSDisconnectedException)
            {
                // Circuit disconnected — safe to ignore
            }
#endif
            catch (Exception)
            {
                // Fallback for net5.0 where JSDisconnectedException doesn't exist
            }
        }

        GC.SuppressFinalize(this);
    }
}
