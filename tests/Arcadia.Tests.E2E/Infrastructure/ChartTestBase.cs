using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace Arcadia.Tests.E2E.Infrastructure;

public class ChartTestBase : PageTest
{
    private static readonly string ScreenshotDir = Path.Combine(
        FindRepoRoot(), "tests", "Arcadia.Tests.E2E", "Screenshots");

    protected async Task NavigateToChart(string chartType)
    {
        await Page.GotoAsync($"{TestConstants.BaseUrl}{TestConstants.TestPagePrefix}/{chartType}",
            new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });
        await Page.WaitForSelectorAsync(TestConstants.Selectors.ChartContainer,
            new PageWaitForSelectorOptions { State = WaitForSelectorState.Visible });
        // Wait for animations to settle
        await Page.WaitForTimeoutAsync(TestConstants.ChartRenderWaitMs);
    }

    protected async Task EnableReducedMotion()
    {
        await Page.EmulateMediaAsync(new PageEmulateMediaOptions
        {
            ReducedMotion = ReducedMotion.Reduce
        });
    }

    protected async Task SetViewport(int width, int height)
    {
        await Page.SetViewportSizeAsync(width, height);
        await Page.WaitForTimeoutAsync(500);
    }

    protected ILocator ChartContainer => Page.Locator(TestConstants.Selectors.ChartContainer).First;
    protected ILocator ChartSvg => Page.Locator(TestConstants.Selectors.ChartSvg).First;
    protected ILocator Legend => Page.Locator(TestConstants.Selectors.Legend).First;
    protected ILocator SrTable => Page.Locator(TestConstants.Selectors.SrTable).First;

    /// <summary>
    /// Takes a screenshot of the chart container and compares against a stored baseline.
    /// On first run (no baseline), saves the screenshot as the new baseline.
    /// Set env var UPDATE_SNAPSHOTS=1 to overwrite existing baselines.
    /// </summary>
    protected async Task AssertChartScreenshot(string name, double maxDiffPercent = 1.0)
    {
        var baselinePath = Path.Combine(ScreenshotDir, "baselines", name);
        var actualPath = Path.Combine(ScreenshotDir, "actual", name);

        Directory.CreateDirectory(Path.GetDirectoryName(baselinePath)!);
        Directory.CreateDirectory(Path.GetDirectoryName(actualPath)!);

        // Take screenshot of chart container element
        var screenshot = await ChartContainer.ScreenshotAsync(new LocatorScreenshotOptions
        {
            Type = ScreenshotType.Png
        });

        var updateSnapshots = Environment.GetEnvironmentVariable("UPDATE_SNAPSHOTS") == "1";

        if (!File.Exists(baselinePath) || updateSnapshots)
        {
            await File.WriteAllBytesAsync(baselinePath, screenshot);
            Console.WriteLine($"[Baseline saved] {name}");
            return; // First run — baseline captured, test passes
        }

        // Save actual for diff review on failure
        await File.WriteAllBytesAsync(actualPath, screenshot);

        // Compare file sizes as a quick sanity check (exact pixel comparison
        // would require image processing libs — size + byte diff is sufficient
        // for catching major layout regressions)
        var baseline = await File.ReadAllBytesAsync(baselinePath);
        var diffBytes = CountDiffBytes(baseline, screenshot);
        var totalBytes = Math.Max(baseline.Length, screenshot.Length);
        var diffPercent = totalBytes > 0 ? (diffBytes * 100.0 / totalBytes) : 0;

        if (diffPercent > maxDiffPercent)
        {
            Assert.Fail(
                $"Screenshot '{name}' differs from baseline by {diffPercent:F1}% ({diffBytes} of {totalBytes} bytes). " +
                $"Actual: {actualPath}. Baseline: {baselinePath}. " +
                $"Set UPDATE_SNAPSHOTS=1 to update baselines.");
        }

        // Clean up actual file on success
        if (File.Exists(actualPath)) File.Delete(actualPath);
    }

    private static int CountDiffBytes(byte[] a, byte[] b)
    {
        var maxLen = Math.Max(a.Length, b.Length);
        var minLen = Math.Min(a.Length, b.Length);
        var diff = maxLen - minLen; // Extra bytes are all different
        for (var i = 0; i < minLen; i++)
        {
            if (a[i] != b[i]) diff++;
        }
        return diff;
    }

    private static string FindRepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            if (File.Exists(Path.Combine(dir.FullName, "HelixUI.sln")))
                return dir.FullName;
            dir = dir.Parent;
        }
        // Fallback
        return Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..");
    }
}
