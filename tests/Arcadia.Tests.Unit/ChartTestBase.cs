using Bunit;

namespace Arcadia.Tests.Unit;

/// <summary>
/// Base class for all chart unit tests. Configures JSInterop in loose mode
/// so responsive charts (Width=0) don't crash when trying to call ResizeObserver.
/// </summary>
public abstract class ChartTestBase : BunitContext
{
    protected ChartTestBase()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }
}
