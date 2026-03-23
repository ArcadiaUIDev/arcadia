using Arcadia.Tests.E2E.Infrastructure;

namespace Arcadia.Tests.E2E.CrossCutting;

[TestFixture]
public class DarkModeTests : ChartTestBase
{
    private static IEnumerable<TestCaseData> AllCharts() =>
        TestConstants.AllChartTypes.Select(c => new TestCaseData(c).SetName($"{c}_DarkMode"));

    [TestCaseSource(nameof(AllCharts))]
    public async Task Chart_DarkMode_MatchesBaseline(string chartType)
    {
        await EnableReducedMotion();
        // Emulate dark color scheme — charts use currentColor which inherits
        await Page.EmulateMediaAsync(new() { ColorScheme = Microsoft.Playwright.ColorScheme.Dark });
        await NavigateToChart(chartType);
        await AssertChartScreenshot($"{chartType}-dark.png");
    }
}
