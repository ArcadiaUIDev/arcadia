using Arcadia.Tests.E2E.Infrastructure;

namespace Arcadia.Tests.E2E.Charts;

[TestFixture]
public class HeatmapTests : ChartTestBase
{
    [Test]
    public async Task Heatmap_DefaultRender_MatchesBaseline()
    {
        await EnableReducedMotion();
        await NavigateToChart("heatmap");
        await AssertChartScreenshot("heatmap-default.png");
    }

    [Test]
    public async Task Heatmap_HasAriaLabel()
    {
        await NavigateToChart("heatmap");
        await Expect(ChartSvg).ToHaveAttributeAsync("role", "figure");
    }

    [Test]
    public async Task Heatmap_HasScreenReaderTable()
    {
        await NavigateToChart("heatmap");
        await Expect(SrTable).ToBeAttachedAsync();
    }

    [Test]
    public async Task Heatmap_HasCells()
    {
        await EnableReducedMotion();
        await NavigateToChart("heatmap");
        var cells = Page.Locator(".arcadia-heatmap__cell");
        var count = await cells.CountAsync();
        Assert.That(count, Is.EqualTo(63)); // 7 days × 9 hours
    }
}
