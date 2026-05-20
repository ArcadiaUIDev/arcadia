using Bunit;
using FluentAssertions;
using Arcadia.Charts.Components.Charts;
using Arcadia.Charts.Core;
using Xunit;

namespace Arcadia.Tests.Unit.Charts;

public record StackedBarRow(string Category, double A, double B);

public class StackedBarChartTests : ChartTestBase
{
    private static readonly List<StackedBarRow> BasicData = new()
    {
        new("Q1", 30, 20), new("Q2", 45, 25), new("Q3", 50, 30), new("Q4", 60, 35),
    };

    private static readonly List<SeriesConfig<StackedBarRow>> BasicSeries = new()
    {
        new() { Name = "A", Field = d => d.A },
        new() { Name = "B", Field = d => d.B },
    };

    [Fact]
    public void Renders_SvgWithFigureRole()
    {
        var cut = Render<ArcadiaStackedBarChart<StackedBarRow>>(p => p
            .Add(c => c.Data, BasicData)
            .Add(c => c.XField, (Func<StackedBarRow, object>)(d => d.Category))
            .Add(c => c.Series, BasicSeries));

        cut.Find("svg[role='figure']").Should().NotBeNull();
    }

    [Fact]
    public void Renders_ScreenReaderTable()
    {
        var cut = Render<ArcadiaStackedBarChart<StackedBarRow>>(p => p
            .Add(c => c.Data, BasicData)
            .Add(c => c.XField, (Func<StackedBarRow, object>)(d => d.Category))
            .Add(c => c.Series, BasicSeries));

        cut.Find("table.arcadia-sr-only").Should().NotBeNull();
    }

    [Fact]
    public void DefaultsToStacked_WhenNotExplicitlySet()
    {
        // ArcadiaStackedBarChart sets Stacked=true by default; ArcadiaBarChart defaults to false.
        var cut = Render<ArcadiaStackedBarChart<StackedBarRow>>(p => p
            .Add(c => c.Data, BasicData)
            .Add(c => c.XField, (Func<StackedBarRow, object>)(d => d.Category))
            .Add(c => c.Series, BasicSeries));

        cut.Instance.Stacked.Should().BeTrue();
    }

    [Fact]
    public void RespectsExplicitStackedFalse()
    {
        var cut = Render<ArcadiaStackedBarChart<StackedBarRow>>(p => p
            .Add(c => c.Data, BasicData)
            .Add(c => c.XField, (Func<StackedBarRow, object>)(d => d.Category))
            .Add(c => c.Series, BasicSeries)
            .Add(c => c.Stacked, false));

        cut.Instance.Stacked.Should().BeFalse();
    }

    [Fact]
    public void Renders_NoData_WhenDataIsNull()
    {
        var cut = Render<ArcadiaStackedBarChart<StackedBarRow>>(p => p
            .Add(c => c.Data, (IReadOnlyList<StackedBarRow>?)null)
            .Add(c => c.XField, (Func<StackedBarRow, object>)(d => d.Category))
            .Add(c => c.Series, BasicSeries));

        cut.Markup.Should().Contain("No data available");
    }
}
