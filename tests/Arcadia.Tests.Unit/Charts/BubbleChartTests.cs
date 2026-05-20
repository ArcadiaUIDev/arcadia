using Bunit;
using FluentAssertions;
using Arcadia.Charts.Components.Charts;
using Xunit;

namespace Arcadia.Tests.Unit.Charts;

public record BubbleData(double X, double Y, double Size);

public class BubbleChartTests : ChartTestBase
{
    private static readonly List<BubbleData> BasicData = new()
    {
        new(1, 10, 5), new(2, 20, 8), new(3, 15, 12), new(4, 25, 6),
    };

    [Fact]
    public void Renders_SvgWithFigureRole()
    {
        var cut = Render<ArcadiaBubbleChart<BubbleData>>(p => p
            .Add(c => c.Data, BasicData)
            .Add(c => c.XField, (Func<BubbleData, double>)(d => d.X))
            .Add(c => c.YField, (Func<BubbleData, double>)(d => d.Y))
            .Add(c => c.SizeField, (Func<BubbleData, double>)(d => d.Size)));

        cut.Find("svg[role='figure']").Should().NotBeNull();
    }

    [Fact]
    public void Renders_ScreenReaderTable()
    {
        var cut = Render<ArcadiaBubbleChart<BubbleData>>(p => p
            .Add(c => c.Data, BasicData)
            .Add(c => c.XField, (Func<BubbleData, double>)(d => d.X))
            .Add(c => c.YField, (Func<BubbleData, double>)(d => d.Y))
            .Add(c => c.SizeField, (Func<BubbleData, double>)(d => d.Size)));

        cut.Find("table.arcadia-sr-only").Should().NotBeNull();
    }

    [Fact]
    public void InheritsScatterChartBehavior_RendersWithoutSizeField()
    {
        // BubbleChart inherits from ScatterChart; SizeField is the bubble-specific addition,
        // but the chart should still render without it (degrades to plain scatter).
        var cut = Render<ArcadiaBubbleChart<BubbleData>>(p => p
            .Add(c => c.Data, BasicData)
            .Add(c => c.XField, (Func<BubbleData, double>)(d => d.X))
            .Add(c => c.YField, (Func<BubbleData, double>)(d => d.Y)));

        cut.Find("svg[role='figure']").Should().NotBeNull();
    }

    [Fact]
    public void Renders_NoData_WhenDataIsNull()
    {
        var cut = Render<ArcadiaBubbleChart<BubbleData>>(p => p
            .Add(c => c.Data, (IReadOnlyList<BubbleData>?)null)
            .Add(c => c.XField, (Func<BubbleData, double>)(d => d.X))
            .Add(c => c.YField, (Func<BubbleData, double>)(d => d.Y)));

        cut.Markup.Should().Contain("No data available");
    }
}
