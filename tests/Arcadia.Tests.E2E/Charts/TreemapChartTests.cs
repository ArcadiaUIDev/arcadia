using Arcadia.Tests.E2E.Infrastructure;

namespace Arcadia.Tests.E2E.Charts;

[TestFixture]
public class TreemapChartTests : ChartTestBase
{
    [Test]
    public async Task TreemapChart_DefaultRender_MatchesBaseline()
    {
        await EnableReducedMotion();
        await NavigateToChart("treemap");
        await AssertChartScreenshot("treemap-default.png");
    }

    [Test]
    public async Task TreemapChart_HasAriaLabel()
    {
        await NavigateToChart("treemap");
        await Expect(ChartSvg).ToHaveAttributeAsync("role", "figure");
    }

    [Test]
    public async Task TreemapChart_HasScreenReaderTable()
    {
        await NavigateToChart("treemap");
        await Expect(SrTable).ToBeAttachedAsync();
        var rows = SrTable.Locator("tbody tr");
        await Expect(rows).ToHaveCountAsync(6);
    }

    [Test]
    public async Task TreemapChart_HasSixCells()
    {
        await EnableReducedMotion();
        await NavigateToChart("treemap");
        var cells = Page.Locator("rect.arcadia-chart__bar, rect.arcadia-treemap__cell");
        var count = await cells.CountAsync();
        Assert.That(count, Is.GreaterThanOrEqualTo(6));
    }
}
