using Arcadia.Tests.E2E.Infrastructure;

namespace Arcadia.Tests.E2E.Charts;

[TestFixture]
public class GaugeChartTests : ChartTestBase
{
    [Test]
    public async Task GaugeChart_DefaultRender_MatchesBaseline()
    {
        await EnableReducedMotion();
        await NavigateToChart("gauge");
        await AssertChartScreenshot("gauge-default.png");
    }

    [Test]
    public async Task GaugeChart_HasThreeGauges()
    {
        await NavigateToChart("gauge");
        var gauges = Page.Locator(".arcadia-chart");
        await Expect(gauges).ToHaveCountAsync(3);
    }

    [Test]
    public async Task GaugeChart_HasAriaLabel()
    {
        await NavigateToChart("gauge");
        var svg = Page.Locator("svg[data-chart]").First;
        await Expect(svg).ToHaveAttributeAsync("role", "figure");
    }

    [Test]
    public async Task GaugeChart_HasArcPaths()
    {
        await EnableReducedMotion();
        await NavigateToChart("gauge");
        var paths = Page.Locator("path");
        var count = await paths.CountAsync();
        Assert.That(count, Is.GreaterThan(3));
    }
}
