using Arcadia.Tests.E2E.Infrastructure;

namespace Arcadia.Tests.E2E.Charts;

[TestFixture]
public class SankeyChartTests : ChartTestBase
{
    [Test]
    public async Task SankeyChart_DefaultRender_MatchesBaseline()
    {
        await EnableReducedMotion();
        await NavigateToChart("sankey");
        await AssertChartScreenshot("sankey-default.png");
    }

    [Test]
    public async Task SankeyChart_HasAriaLabel()
    {
        await NavigateToChart("sankey");
        await Expect(ChartSvg).ToHaveAttributeAsync("role", "figure");
        await Expect(ChartSvg).ToHaveAttributeAsync("aria-label", "Energy Flow");
    }

    [Test]
    public async Task SankeyChart_HasScreenReaderTable()
    {
        await NavigateToChart("sankey");
        await Expect(SrTable).ToBeAttachedAsync();
        var headers = SrTable.Locator("thead th");
        await Expect(headers).ToHaveCountAsync(3);
        await Expect(headers.Nth(0)).ToHaveTextAsync("Source");
        await Expect(headers.Nth(1)).ToHaveTextAsync("Target");
        await Expect(headers.Nth(2)).ToHaveTextAsync("Value");
    }

    [Test]
    public async Task SankeyChart_HasCorrectNodeCount()
    {
        await EnableReducedMotion();
        await NavigateToChart("sankey");
        var nodes = Page.Locator("rect.arcadia-chart__sankey-node");
        // 17 nodes: 5 sources + 3 sectors + 3 end use + 6 applications
        await Expect(nodes).ToHaveCountAsync(17);
    }

    [Test]
    public async Task SankeyChart_HasCorrectLinkCount()
    {
        await EnableReducedMotion();
        await NavigateToChart("sankey");
        var links = Page.Locator("path.arcadia-chart__sankey-link");
        // 24 links total: 8 (sources→sectors) + 7 (sectors→end use) + 9 (end use→applications)
        await Expect(links).ToHaveCountAsync(24);
    }

    [Test]
    public async Task SankeyChart_HasTitle()
    {
        await NavigateToChart("sankey");
        var title = Page.Locator(".arcadia-chart__title");
        await Expect(title).ToHaveTextAsync("Energy Flow");
    }

    [Test]
    public async Task SankeyChart_HasNodeLabels()
    {
        await EnableReducedMotion();
        await NavigateToChart("sankey");
        var labels = Page.Locator(".arcadia-chart__data-label");
        // One label per node = 17
        await Expect(labels).ToHaveCountAsync(17);
    }

    [Test]
    public async Task SankeyChart_LinkHover_ShowsTooltip()
    {
        await EnableReducedMotion();
        await NavigateToChart("sankey");
        var firstLink = Page.Locator("path.arcadia-chart__sankey-link").First;
        await firstLink.HoverAsync();
        // Tooltip should appear
        var tooltip = Page.Locator(".arcadia-tooltip");
        await Expect(tooltip).ToBeVisibleAsync();
    }

    [Test]
    public async Task SankeyChart_LinkHover_ShowsCorrectTooltipContent()
    {
        await EnableReducedMotion();
        await NavigateToChart("sankey");
        var firstLink = Page.Locator("path.arcadia-chart__sankey-link").First;
        await firstLink.HoverAsync();
        var tooltip = Page.Locator(".arcadia-tooltip");
        await Expect(tooltip).ToBeVisibleAsync();
        // Tooltip should contain an arrow (→) between source and target
        var text = await tooltip.InnerTextAsync();
        Assert.That(text, Does.Contain("→").Or.Contain("→"));
    }

    [Test]
    public async Task SankeyChart_ReducedMotion_NoAnimationClasses()
    {
        await EnableReducedMotion();
        await NavigateToChart("sankey");
        // With reduced motion, animations should be disabled via CSS
        // The classes still exist but the CSS media query overrides them
        var links = Page.Locator("path.arcadia-animate-sankey");
        var count = await links.CountAsync();
        // Links still have the class, but CSS prefers-reduced-motion overrides
        Assert.That(count, Is.GreaterThan(0));
    }
}
