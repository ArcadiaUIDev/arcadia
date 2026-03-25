using Bunit;
using FluentAssertions;
using Arcadia.Charts.Components.Charts;
using Arcadia.Charts.Core;
using Xunit;

namespace Arcadia.Tests.Unit.Charts;

public record TimePoint(DateTime Timestamp, double Value);

public class LineChartTimeAxisTests : ChartTestBase
{
    private static readonly List<TimePoint> RegularData = new()
    {
        new(new DateTime(2026, 1, 1), 100),
        new(new DateTime(2026, 2, 1), 120),
        new(new DateTime(2026, 3, 1), 110),
        new(new DateTime(2026, 4, 1), 140),
        new(new DateTime(2026, 5, 1), 130),
        new(new DateTime(2026, 6, 1), 150),
    };

    private static readonly List<TimePoint> IrregularData = new()
    {
        new(new DateTime(2026, 1, 1), 100),
        new(new DateTime(2026, 1, 2), 105),
        new(new DateTime(2026, 1, 3), 108),
        // Big gap
        new(new DateTime(2026, 3, 1), 200),
        new(new DateTime(2026, 3, 2), 195),
    };

    private static readonly List<SeriesConfig<TimePoint>> DefaultSeries = new()
    {
        new() { Name = "Values", Field = d => d.Value },
    };

    [Fact]
    public void TimeAxis_RendersSvg()
    {
        var cut = Render<ArcadiaLineChart<TimePoint>>(p => p
            .Add(c => c.Data, RegularData)
            .Add(c => c.XField, (Func<TimePoint, object>)(d => d.Timestamp))
            .Add(c => c.Series, DefaultSeries)
            .Add(c => c.XAxisType, "time"));

        cut.Find("svg[data-chart]").Should().NotBeNull();
    }

    [Fact]
    public void TimeAxis_RendersPaths()
    {
        var cut = Render<ArcadiaLineChart<TimePoint>>(p => p
            .Add(c => c.Data, RegularData)
            .Add(c => c.XField, (Func<TimePoint, object>)(d => d.Timestamp))
            .Add(c => c.Series, DefaultSeries)
            .Add(c => c.XAxisType, "time"));

        cut.FindAll("path.arcadia-chart__line").Count.Should().BeGreaterOrEqualTo(1);
    }

    [Fact]
    public void TimeAxis_GeneratesTimeFormattedLabels()
    {
        var cut = Render<ArcadiaLineChart<TimePoint>>(p => p
            .Add(c => c.Data, RegularData)
            .Add(c => c.XField, (Func<TimePoint, object>)(d => d.Timestamp))
            .Add(c => c.Series, DefaultSeries)
            .Add(c => c.XAxisType, "time"));

        // 6-month range should produce "MMM yyyy" or "MMM" formatted labels
        var markup = cut.Markup;
        // Should contain month abbreviations, not raw DateTime.ToString()
        markup.Should().NotContain("1/1/2026");
    }

    [Fact]
    public void TimeAxis_RespectXAxisFormatString()
    {
        var cut = Render<ArcadiaLineChart<TimePoint>>(p => p
            .Add(c => c.Data, RegularData)
            .Add(c => c.XField, (Func<TimePoint, object>)(d => d.Timestamp))
            .Add(c => c.Series, DefaultSeries)
            .Add(c => c.XAxisType, "time")
            .Add(c => c.XAxisFormatString, "yyyy-MM"));

        // Custom format should appear in tick labels
        cut.Markup.Should().Contain("2026-01");
    }

    [Fact]
    public void TimeAxis_IrregularSpacing_RendersWithoutError()
    {
        var cut = Render<ArcadiaLineChart<TimePoint>>(p => p
            .Add(c => c.Data, IrregularData)
            .Add(c => c.XField, (Func<TimePoint, object>)(d => d.Timestamp))
            .Add(c => c.Series, DefaultSeries)
            .Add(c => c.XAxisType, "time"));

        cut.Find("svg[data-chart]").Should().NotBeNull();
        cut.FindAll("path.arcadia-chart__line").Count.Should().BeGreaterOrEqualTo(1);
    }

    [Fact]
    public void CategoryAxis_RemainsDefault()
    {
        // Verify no regression: default XAxisType="category" still works
        var data = new List<TimePoint>
        {
            new(DateTime.Now, 100), new(DateTime.Now.AddDays(1), 120),
            new(DateTime.Now.AddDays(2), 110),
        };

        var cut = Render<ArcadiaLineChart<TimePoint>>(p => p
            .Add(c => c.Data, data)
            .Add(c => c.XField, (Func<TimePoint, object>)(d => d.Timestamp))
            .Add(c => c.Series, DefaultSeries));

        // Default is "category", should render without errors
        cut.Find("svg[data-chart]").Should().NotBeNull();
    }

    [Fact]
    public void TimeAxis_WithTitle()
    {
        var cut = Render<ArcadiaLineChart<TimePoint>>(p => p
            .Add(c => c.Data, RegularData)
            .Add(c => c.XField, (Func<TimePoint, object>)(d => d.Timestamp))
            .Add(c => c.Series, DefaultSeries)
            .Add(c => c.XAxisType, "time")
            .Add(c => c.Title, "Time Series"));

        cut.Find(".arcadia-chart__title").TextContent.Should().Be("Time Series");
    }

    [Fact]
    public void TimeAxis_ShowsPoints()
    {
        var cut = Render<ArcadiaLineChart<TimePoint>>(p => p
            .Add(c => c.Data, RegularData)
            .Add(c => c.XField, (Func<TimePoint, object>)(d => d.Timestamp))
            .Add(c => c.Series, DefaultSeries)
            .Add(c => c.XAxisType, "time")
            .Add(c => c.ShowPoints, true));

        cut.FindAll("circle").Count.Should().Be(6); // 6 data points
    }
}

public class TimeTickLabelFormatterTests
{
    [Fact]
    public void UnderTwoHours_ReturnsSeconds()
    {
        var fmt = Arcadia.Charts.Core.Layout.TimeTickLabelFormatter.GetFormat(TimeSpan.FromMinutes(30));
        fmt.Should().Be("HH:mm:ss");
    }

    [Fact]
    public void UnderTwoDays_ReturnsHoursMinutes()
    {
        var fmt = Arcadia.Charts.Core.Layout.TimeTickLabelFormatter.GetFormat(TimeSpan.FromHours(12));
        fmt.Should().Be("HH:mm");
    }

    [Fact]
    public void UnderSixtyDays_ReturnsMMMd()
    {
        var fmt = Arcadia.Charts.Core.Layout.TimeTickLabelFormatter.GetFormat(TimeSpan.FromDays(30));
        fmt.Should().Be("MMM d");
    }

    [Fact]
    public void UnderTwoYears_ReturnsMMMyyy()
    {
        var fmt = Arcadia.Charts.Core.Layout.TimeTickLabelFormatter.GetFormat(TimeSpan.FromDays(365));
        fmt.Should().Be("MMM yyyy");
    }

    [Fact]
    public void OverTwoYears_ReturnsYear()
    {
        var fmt = Arcadia.Charts.Core.Layout.TimeTickLabelFormatter.GetFormat(TimeSpan.FromDays(1000));
        fmt.Should().Be("yyyy");
    }
}
