using Arcadia.Tests.E2E.Infrastructure;

namespace Arcadia.Tests.E2E.CrossCutting;

[TestFixture]
public class ResponsiveTests : ChartTestBase
{
    private static IEnumerable<TestCaseData> ViewportCases()
    {
        var viewports = new[]
        {
            ("Mobile", TestConstants.Viewports.Mobile),
            ("Tablet", TestConstants.Viewports.Tablet),
            ("Desktop", TestConstants.Viewports.Desktop)
        };

        foreach (var chart in TestConstants.AllChartTypes)
        {
            foreach (var (name, vp) in viewports)
            {
                yield return new TestCaseData(chart, vp.Width, vp.Height)
                    .SetName($"{chart}_{name}_{vp.Width}x{vp.Height}");
            }
        }
    }

    [TestCaseSource(nameof(ViewportCases))]
    public async Task Chart_AtViewport_MatchesBaseline(string chartType, int width, int height)
    {
        await EnableReducedMotion();
        await SetViewport(width, height);
        await NavigateToChart(chartType);
        await AssertChartScreenshot($"{chartType}-{width}x{height}.png");
    }
}
