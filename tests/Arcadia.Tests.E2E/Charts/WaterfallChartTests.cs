using Arcadia.Tests.E2E.Infrastructure;

namespace Arcadia.Tests.E2E.Charts;

[TestFixture]
public class WaterfallChartTests : ChartTestBase
{
    [Test]
    public async Task WaterfallChart_DefaultRender_MatchesBaseline()
    {
        await EnableReducedMotion();
        await NavigateToChart("waterfall");
        await AssertChartScreenshot("waterfall-default.png");
    }

    [Test]
    public async Task WaterfallChart_HasAriaLabel()
    {
        await NavigateToChart("waterfall");
        await Expect(ChartSvg).ToHaveAttributeAsync("role", "figure");
    }

    [Test]
    public async Task WaterfallChart_HasScreenReaderTable()
    {
        await NavigateToChart("waterfall");
        await Expect(SrTable).ToBeAttachedAsync();
        var rows = SrTable.Locator("tbody tr");
        await Expect(rows).ToHaveCountAsync(5);
    }

    [Test]
    public async Task WaterfallChart_HasBars()
    {
        await EnableReducedMotion();
        await NavigateToChart("waterfall");
        var bars = Page.Locator("rect.arcadia-chart__bar");
        var count = await bars.CountAsync();
        Assert.That(count, Is.EqualTo(5));
    }
}
