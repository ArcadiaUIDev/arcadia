using Bunit;
using FluentAssertions;
using Arcadia.Charts.Components.Charts;
using Xunit;

namespace Arcadia.Tests.Unit.Charts;

public record DonutData(string Name, double Value);

public class DonutChartTests : ChartTestBase
{
    private static readonly List<DonutData> BasicData = new()
    {
        new("A", 40), new("B", 30), new("C", 20), new("D", 10),
    };

    [Fact]
    public void Renders_SvgWithFigureRole()
    {
        var cut = Render<ArcadiaDonutChart<DonutData>>(p => p
            .Add(c => c.Data, BasicData)
            .Add(c => c.NameField, (Func<DonutData, string>)(d => d.Name))
            .Add(c => c.ValueField, (Func<DonutData, double>)(d => d.Value)));

        cut.Find("svg[role='figure']").Should().NotBeNull();
    }

    [Fact]
    public void Renders_ScreenReaderTable()
    {
        var cut = Render<ArcadiaDonutChart<DonutData>>(p => p
            .Add(c => c.Data, BasicData)
            .Add(c => c.NameField, (Func<DonutData, string>)(d => d.Name))
            .Add(c => c.ValueField, (Func<DonutData, double>)(d => d.Value)));

        cut.Find("table.arcadia-sr-only").Should().NotBeNull();
    }

    [Fact]
    public void DefaultsToInnerRadius055_WhenNotExplicitlySet()
    {
        // ArcadiaDonutChart sets InnerRadius = 0.55 by default; ArcadiaPieChart defaults to 0 (pie).
        var cut = Render<ArcadiaDonutChart<DonutData>>(p => p
            .Add(c => c.Data, BasicData)
            .Add(c => c.NameField, (Func<DonutData, string>)(d => d.Name))
            .Add(c => c.ValueField, (Func<DonutData, double>)(d => d.Value)));

        cut.Instance.InnerRadius.Should().BeApproximately(0.55, 0.001);
    }

    [Fact]
    public void RespectsExplicitInnerRadius()
    {
        var cut = Render<ArcadiaDonutChart<DonutData>>(p => p
            .Add(c => c.Data, BasicData)
            .Add(c => c.NameField, (Func<DonutData, string>)(d => d.Name))
            .Add(c => c.ValueField, (Func<DonutData, double>)(d => d.Value))
            .Add(c => c.InnerRadius, 0.30));

        cut.Instance.InnerRadius.Should().BeApproximately(0.30, 0.001);
    }

    [Fact]
    public void Renders_NoData_WhenDataIsNull()
    {
        var cut = Render<ArcadiaDonutChart<DonutData>>(p => p
            .Add(c => c.Data, (IReadOnlyList<DonutData>?)null)
            .Add(c => c.NameField, (Func<DonutData, string>)(d => d.Name))
            .Add(c => c.ValueField, (Func<DonutData, double>)(d => d.Value)));

        cut.Markup.Should().Contain("No data available");
    }
}
