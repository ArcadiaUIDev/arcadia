using Arcadia.Tests.E2E.Infrastructure;

namespace Arcadia.Tests.E2E.Charts;

[TestFixture]
public class BarChartTests : ChartTestBase
{
    [Test]
    public async Task BarChart_DefaultRender_MatchesBaseline()
    {
        await EnableReducedMotion();
        await NavigateToChart("bar");
        await AssertChartScreenshot("bar-default.png");
    }

    [Test]
    public async Task BarChart_HasAriaLabel()
    {
        await NavigateToChart("bar");
        await Expect(ChartSvg).ToHaveAttributeAsync("role", "figure");
    }

    [Test]
    public async Task BarChart_HasScreenReaderTable()
    {
        await NavigateToChart("bar");
        await Expect(SrTable).ToBeAttachedAsync();
        var rows = SrTable.Locator("tbody tr");
        await Expect(rows).ToHaveCountAsync(4);
    }

    [Test]
    public async Task BarChart_HasLegend_WithThreeSeries()
    {
        await NavigateToChart("bar");
        var buttons = Legend.Locator(".arcadia-chart__legend-btn");
        await Expect(buttons).ToHaveCountAsync(3);
    }

    [Test]
    public async Task BarChart_HasBars()
    {
        await EnableReducedMotion();
        await NavigateToChart("bar");
        var bars = Page.Locator("rect.arcadia-chart__bar");
        var count = await bars.CountAsync();
        Assert.That(count, Is.EqualTo(12)); // 4 quarters × 3 regions
    }
}
