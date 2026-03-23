using Arcadia.Tests.E2E.Infrastructure;

namespace Arcadia.Tests.E2E.CrossCutting;

/// <summary>
/// Regression tests for label overlap — the candlestick label bug and similar
/// cramped-layout issues. These use narrow viewports to stress-test the layout engine.
/// </summary>
[TestFixture]
public class LabelOverlapTests : ChartTestBase
{
    [Test]
    public async Task CandlestickChart_NarrowViewport_LabelsReadable()
    {
        await EnableReducedMotion();
        await SetViewport(400, 600);
        await NavigateToChart("candlestick");
        await AssertChartScreenshot("candlestick-narrow-400.png");
    }

    [Test]
    public async Task LineChart_NarrowViewport_XLabelsDoNotOverlap()
    {
        await EnableReducedMotion();
        await SetViewport(375, 600);
        await NavigateToChart("line");
        await AssertChartScreenshot("line-narrow-375.png");
    }

    [Test]
    public async Task BarChart_NarrowViewport_CategoriesReadable()
    {
        await EnableReducedMotion();
        await SetViewport(400, 600);
        await NavigateToChart("bar");
        await AssertChartScreenshot("bar-narrow-400.png");
    }

    [Test]
    public async Task Heatmap_NarrowViewport_AxisLabelsVisible()
    {
        await EnableReducedMotion();
        await SetViewport(375, 600);
        await NavigateToChart("heatmap");
        await AssertChartScreenshot("heatmap-narrow-375.png");
    }

    [Test]
    public async Task CandlestickChart_HasTickLabels()
    {
        await EnableReducedMotion();
        await NavigateToChart("candlestick");
        var labels = Page.Locator(TestConstants.Selectors.TickLabel);
        var count = await labels.CountAsync();
        Assert.That(count, Is.GreaterThan(0), "Candlestick chart should have visible tick labels");
    }
}
