using Arcadia.Tests.E2E.Infrastructure;

namespace Arcadia.Tests.E2E.Charts;

[TestFixture]
public class ScatterChartTests : ChartTestBase
{
    [Test]
    public async Task ScatterChart_DefaultRender_MatchesBaseline()
    {
        await EnableReducedMotion();
        await NavigateToChart("scatter");
        await AssertChartScreenshot("scatter-default.png");
    }

    [Test]
    public async Task ScatterChart_HasAriaLabel()
    {
        await NavigateToChart("scatter");
        await Expect(ChartSvg).ToHaveAttributeAsync("role", "figure");
    }

    [Test]
    public async Task ScatterChart_HasScreenReaderTable()
    {
        await NavigateToChart("scatter");
        await Expect(SrTable).ToBeAttachedAsync();
    }

    [Test]
    public async Task ScatterChart_HasFiftyPoints()
    {
        await EnableReducedMotion();
        await NavigateToChart("scatter");
        var points = Page.Locator("circle.arcadia-chart__point");
        await Expect(points).ToHaveCountAsync(50);
    }

    [Test]
    public async Task ScatterChart_HasGridLines()
    {
        await NavigateToChart("scatter");
        var gridLines = Page.Locator("line[stroke-dasharray='4,4']");
        var count = await gridLines.CountAsync();
        Assert.That(count, Is.GreaterThan(0));
    }
}
