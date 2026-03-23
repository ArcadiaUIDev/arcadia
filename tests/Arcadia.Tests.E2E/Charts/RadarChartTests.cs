using Arcadia.Tests.E2E.Infrastructure;

namespace Arcadia.Tests.E2E.Charts;

[TestFixture]
public class RadarChartTests : ChartTestBase
{
    [Test]
    public async Task RadarChart_DefaultRender_MatchesBaseline()
    {
        await EnableReducedMotion();
        await NavigateToChart("radar");
        await AssertChartScreenshot("radar-default.png");
    }

    [Test]
    public async Task RadarChart_HasAriaLabel()
    {
        await NavigateToChart("radar");
        await Expect(ChartSvg).ToHaveAttributeAsync("role", "figure");
    }

    [Test]
    public async Task RadarChart_HasScreenReaderTable()
    {
        await NavigateToChart("radar");
        await Expect(SrTable).ToBeAttachedAsync();
        var rows = SrTable.Locator("tbody tr");
        await Expect(rows).ToHaveCountAsync(6);
    }

    [Test]
    public async Task RadarChart_HasLegend_WithTwoTeams()
    {
        await NavigateToChart("radar");
        var items = Legend.Locator(".arcadia-chart__legend-item");
        await Expect(items).ToHaveCountAsync(2);
    }

    [Test]
    public async Task RadarChart_HasPolygonPaths()
    {
        await EnableReducedMotion();
        await NavigateToChart("radar");
        var polygons = Page.Locator("polygon");
        var count = await polygons.CountAsync();
        Assert.That(count, Is.GreaterThan(0));
    }
}
