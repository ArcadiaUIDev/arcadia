using Arcadia.Tests.E2E.Infrastructure;

namespace Arcadia.Tests.E2E.Charts;

[TestFixture]
public class FunnelChartTests : ChartTestBase
{
    [Test]
    public async Task FunnelChart_DefaultRender_MatchesBaseline()
    {
        await EnableReducedMotion();
        await NavigateToChart("funnel");
        await AssertChartScreenshot("funnel-default.png");
    }

    [Test]
    public async Task FunnelChart_HasAriaLabel()
    {
        await NavigateToChart("funnel");
        await Expect(ChartSvg).ToHaveAttributeAsync("role", "figure");
    }

    [Test]
    public async Task FunnelChart_HasScreenReaderTable()
    {
        await NavigateToChart("funnel");
        await Expect(SrTable).ToBeAttachedAsync();
        var rows = SrTable.Locator("tbody tr");
        await Expect(rows).ToHaveCountAsync(5);
    }

    [Test]
    public async Task FunnelChart_HasFiveStages()
    {
        await EnableReducedMotion();
        await NavigateToChart("funnel");
        var paths = Page.Locator("path.arcadia-chart__bar, path.arcadia-funnel__stage");
        var count = await paths.CountAsync();
        Assert.That(count, Is.GreaterThanOrEqualTo(5));
    }
}
