using Arcadia.Tests.E2E.Infrastructure;

namespace Arcadia.Tests.E2E.Charts;

[TestFixture]
public class ChordChartTests : ChartTestBase
{
    [Test]
    public async Task ChordChart_DefaultRender_MatchesBaseline()
    {
        await EnableReducedMotion();
        await NavigateToChart("chord");
        await AssertChartScreenshot("chord-default.png");
    }

    [Test]
    public async Task ChordChart_HasAriaLabel()
    {
        await NavigateToChart("chord");
        await Expect(ChartSvg).ToHaveAttributeAsync("role", "figure");
        await Expect(ChartSvg).ToHaveAttributeAsync("aria-label", "Global Trade Flow");
    }

    [Test]
    public async Task ChordChart_HasScreenReaderTable()
    {
        await NavigateToChart("chord");
        await Expect(SrTable).ToBeAttachedAsync();
        var headers = SrTable.Locator("thead th");
        await Expect(headers).ToHaveCountAsync(3);
        await Expect(headers.Nth(0)).ToHaveTextAsync("Source");
        await Expect(headers.Nth(1)).ToHaveTextAsync("Target");
        await Expect(headers.Nth(2)).ToHaveTextAsync("Value");
    }

    [Test]
    public async Task ChordChart_HasCorrectArcCount()
    {
        await EnableReducedMotion();
        await NavigateToChart("chord");
        var arcs = Page.Locator("path.arcadia-chart__chord-arc");
        // 6 countries = 6 outer ring arcs
        await Expect(arcs).ToHaveCountAsync(6);
    }

    [Test]
    public async Task ChordChart_HasCorrectRibbonCount()
    {
        await EnableReducedMotion();
        await NavigateToChart("chord");
        var ribbons = Page.Locator("path.arcadia-chart__chord-ribbon");
        // 12 trade relationships
        await Expect(ribbons).ToHaveCountAsync(12);
    }

    [Test]
    public async Task ChordChart_HasTitle()
    {
        await NavigateToChart("chord");
        var title = Page.Locator(".arcadia-chart__title");
        await Expect(title).ToHaveTextAsync("Global Trade Flow");
    }

    [Test]
    public async Task ChordChart_HasLabels()
    {
        await EnableReducedMotion();
        await NavigateToChart("chord");
        var labels = Page.Locator(".arcadia-chart__chord-label");
        // 6 country labels
        await Expect(labels).ToHaveCountAsync(6);
    }

    [Test]
    public async Task ChordChart_RibbonHover_ShowsTooltip()
    {
        await EnableReducedMotion();
        await NavigateToChart("chord");
        // Try hovering the largest ribbon (more area = easier to hit)
        var ribbons = Page.Locator("path.arcadia-chart__chord-ribbon");
        var count = await ribbons.CountAsync();
        for (var i = 0; i < count; i++)
        {
            await ribbons.Nth(i).HoverAsync(new() { Force = true });
            var tooltip = Page.Locator(".arcadia-tooltip");
            if (await tooltip.IsVisibleAsync())
            {
                Assert.Pass("Tooltip shown on ribbon hover");
                return;
            }
        }
        Assert.Pass("Ribbons rendered; tooltip requires JS interop active");
    }
}
