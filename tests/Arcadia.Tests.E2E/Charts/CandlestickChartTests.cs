using Arcadia.Tests.E2E.Infrastructure;

namespace Arcadia.Tests.E2E.Charts;

[TestFixture]
public class CandlestickChartTests : ChartTestBase
{
    [Test]
    public async Task CandlestickChart_DefaultRender_MatchesBaseline()
    {
        await EnableReducedMotion();
        await NavigateToChart("candlestick");
        await AssertChartScreenshot("candlestick-default.png");
    }

    [Test]
    public async Task CandlestickChart_HasAriaLabel()
    {
        await NavigateToChart("candlestick");
        await Expect(ChartSvg).ToHaveAttributeAsync("role", "figure");
    }

    [Test]
    public async Task CandlestickChart_HasScreenReaderTable()
    {
        await NavigateToChart("candlestick");
        await Expect(SrTable).ToBeAttachedAsync();
        var rows = SrTable.Locator("tbody tr");
        await Expect(rows).ToHaveCountAsync(30);
    }

    [Test]
    public async Task CandlestickChart_HasCandleBodies()
    {
        await EnableReducedMotion();
        await NavigateToChart("candlestick");
        var bodies = Page.Locator(".arcadia-candle__body");
        await Expect(bodies).ToHaveCountAsync(30);
    }

    [Test]
    public async Task CandlestickChart_HasOverlayLine()
    {
        await EnableReducedMotion();
        await NavigateToChart("candlestick");
        var overlayLines = Page.Locator("path.arcadia-chart__line");
        var count = await overlayLines.CountAsync();
        Assert.That(count, Is.GreaterThanOrEqualTo(1));
    }
}
