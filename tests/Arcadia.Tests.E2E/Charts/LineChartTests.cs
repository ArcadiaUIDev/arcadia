using Arcadia.Tests.E2E.Infrastructure;

namespace Arcadia.Tests.E2E.Charts;

[TestFixture]
public class LineChartTests : ChartTestBase
{
    [Test]
    public async Task LineChart_DefaultRender_MatchesBaseline()
    {
        await EnableReducedMotion();
        await NavigateToChart("line");
        await AssertChartScreenshot("line-default.png");
    }

    [Test]
    public async Task LineChart_HasAriaLabel()
    {
        await NavigateToChart("line");
        await Expect(ChartSvg).ToHaveAttributeAsync("role", "figure");
        await Expect(ChartSvg).ToHaveAttributeAsync("aria-label", "Monthly Revenue");
    }

    [Test]
    public async Task LineChart_HasScreenReaderTable()
    {
        await NavigateToChart("line");
        await Expect(SrTable).ToBeAttachedAsync();
        var rows = SrTable.Locator("tbody tr");
        await Expect(rows).ToHaveCountAsync(12);
    }

    [Test]
    public async Task LineChart_HasLegend_WithTwoSeries()
    {
        await NavigateToChart("line");
        var buttons = Legend.Locator(".arcadia-chart__legend-btn");
        await Expect(buttons).ToHaveCountAsync(2);
    }

    [Test]
    public async Task LineChart_HasLinePath()
    {
        await EnableReducedMotion();
        await NavigateToChart("line");
        var lines = Page.Locator("path.arcadia-chart__line");
        var count = await lines.CountAsync();
        Assert.That(count, Is.GreaterThanOrEqualTo(1));
    }
}
