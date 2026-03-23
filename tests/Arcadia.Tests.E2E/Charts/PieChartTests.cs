using Arcadia.Tests.E2E.Infrastructure;

namespace Arcadia.Tests.E2E.Charts;

[TestFixture]
public class PieChartTests : ChartTestBase
{
    [Test]
    public async Task PieChart_DefaultRender_MatchesBaseline()
    {
        await EnableReducedMotion();
        await NavigateToChart("pie");
        await AssertChartScreenshot("pie-default.png");
    }

    [Test]
    public async Task PieChart_HasAriaLabel()
    {
        await NavigateToChart("pie");
        await Expect(ChartSvg).ToHaveAttributeAsync("role", "figure");
    }

    [Test]
    public async Task PieChart_HasScreenReaderTable()
    {
        await NavigateToChart("pie");
        await Expect(SrTable).ToBeAttachedAsync();
        var rows = SrTable.Locator("tbody tr");
        await Expect(rows).ToHaveCountAsync(5);
    }

    [Test]
    public async Task PieChart_HasFiveSlices()
    {
        await EnableReducedMotion();
        await NavigateToChart("pie");
        var slices = Page.Locator("path.arcadia-chart__pie-slice");
        await Expect(slices).ToHaveCountAsync(5);
    }

    [Test]
    public async Task PieChart_HasLegend()
    {
        await NavigateToChart("pie");
        await Expect(Legend).ToBeVisibleAsync();
    }
}
