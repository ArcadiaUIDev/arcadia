using Bunit;
using FluentAssertions;
using Arcadia.Charts.Components.Charts;
using Arcadia.Charts.Core;
using Arcadia.DataGrid.Components;
using Xunit;

namespace Arcadia.Tests.Unit.Stress;

/// <summary>
/// Stress tests that verify components handle edge cases gracefully
/// without crashing, throwing, or producing invalid output.
/// </summary>
public class StressTests : ChartTestBase
{
    // ── Null Data ──

    [Fact]
    public void LineChart_NullData_ShowsNoData()
    {
        var cut = Render<ArcadiaLineChart<object>>(p => p
            .Add(c => c.Series, new List<SeriesConfig<object>> { new() { Name = "X", Field = _ => 0 } }));
        cut.FindAll("svg").Count.Should().Be(0);
    }

    [Fact]
    public void BarChart_NullData_ShowsNoData()
    {
        var cut = Render<ArcadiaBarChart<object>>(p => p
            .Add(c => c.Series, new List<SeriesConfig<object>> { new() { Name = "X", Field = _ => 0 } }));
        cut.FindAll("svg").Count.Should().Be(0);
    }

    [Fact]
    public void PieChart_NullData_ShowsNoData()
    {
        var cut = Render<ArcadiaPieChart<object>>(p => p.Add(c => c.Data, (IReadOnlyList<object>?)null));
        cut.FindAll("svg").Count.Should().Be(0);
    }

    [Fact]
    public void SankeyChart_NullData_ShowsNoData()
    {
        var cut = Render<ArcadiaSankeyChart>(p => p.Add(c => c.Data, (IReadOnlyList<SankeyNode>?)null));
        cut.FindAll("svg").Count.Should().Be(0);
    }

    [Fact]
    public void ChordChart_NullData_ShowsNoData()
    {
        var cut = Render<ArcadiaChordChart>(p => p.Add(c => c.Data, (IReadOnlyList<ChordNode>?)null));
        cut.FindAll("svg").Count.Should().Be(0);
    }

    [Fact]
    public void DataGrid_NullData_ShowsEmpty()
    {
        var cut = Render<ArcadiaDataGrid<object>>(p => p.Add(c => c.Data, (IReadOnlyList<object>?)null));
        cut.FindAll("table").Count.Should().Be(0); // no table renders without columns
    }

    // ── Empty Data ──

    [Fact]
    public void LineChart_EmptyData_ShowsNoData()
    {
        var cut = Render<ArcadiaLineChart<object>>(p => p
            .Add(c => c.Data, new List<object>())
            .Add(c => c.Series, new List<SeriesConfig<object>> { new() { Name = "X", Field = _ => 0 } }));
        cut.FindAll("svg").Count.Should().Be(0);
    }

    [Fact]
    public void PieChart_EmptyData_ShowsNoData()
    {
        var cut = Render<ArcadiaPieChart<(string N, double V)>>(p => p
            .Add(c => c.Data, new List<(string N, double V)>())
            .Add(c => c.NameField, (Func<(string N, double V), string>)(d => d.N))
            .Add(c => c.ValueField, (Func<(string N, double V), double>)(d => d.V)));
        cut.FindAll("svg").Count.Should().Be(0);
    }

    [Fact]
    public void SankeyChart_EmptyLinks_ShowsNoData()
    {
        var nodes = new List<SankeyNode> { new() { Id = "a", Label = "A" } };
        var cut = Render<ArcadiaSankeyChart>(p => p
            .Add(c => c.Data, nodes)
            .Add(c => c.Links, new List<SankeyLink>()));
        cut.FindAll("svg").Count.Should().Be(0);
    }

    // ── NaN Values ──

    [Fact]
    public void LineChart_AllNaN_DoesNotCrash()
    {
        var data = new List<(string L, double V)>
        {
            ("A", double.NaN), ("B", double.NaN), ("C", double.NaN)
        };
        var series = new List<SeriesConfig<(string L, double V)>>
        {
            new() { Name = "V", Field = d => d.V }
        };

        // Should not throw
        var cut = Render<ArcadiaLineChart<(string L, double V)>>(p => p
            .Add(c => c.Data, data)
            .Add(c => c.XField, (Func<(string L, double V), object>)(d => d.L))
            .Add(c => c.Series, series));

        // Should render something (even if no visible paths)
        cut.Markup.Should().NotBeEmpty();
    }

    // ── Single Data Point (Line needs 2+) ──

    [Fact]
    public void LineChart_OnePoint_ShowsNoData()
    {
        var data = new List<(string L, double V)> { ("A", 42) };
        var series = new List<SeriesConfig<(string L, double V)>>
        {
            new() { Name = "V", Field = d => d.V }
        };

        var cut = Render<ArcadiaLineChart<(string L, double V)>>(p => p
            .Add(c => c.Data, data)
            .Add(c => c.XField, (Func<(string L, double V), object>)(d => d.L))
            .Add(c => c.Series, series));

        cut.FindAll("svg").Count.Should().Be(0, "Line chart needs 2+ points");
    }

    // ── Circular References (Sankey) ──

    [Fact]
    public void Sankey_CircularRefs_DoesNotCrash()
    {
        var nodes = new List<SankeyNode>
        {
            new() { Id = "a", Label = "A" },
            new() { Id = "b", Label = "B" },
            new() { Id = "c", Label = "C" },
        };
        var links = new List<SankeyLink>
        {
            new() { SourceId = "a", TargetId = "b", Value = 100 },
            new() { SourceId = "b", TargetId = "c", Value = 80 },
            new() { SourceId = "c", TargetId = "a", Value = 60 }, // cycle
        };

        var cut = Render<ArcadiaSankeyChart>(p => p
            .Add(c => c.Data, nodes)
            .Add(c => c.Links, links));

        cut.Find("svg").Should().NotBeNull();
    }

    // ── Zero Values ──

    [Fact]
    public void PieChart_AllZeroValues_DoesNotCrash()
    {
        var data = new List<(string N, double V)>
        {
            ("A", 0), ("B", 0), ("C", 0)
        };

        var cut = Render<ArcadiaPieChart<(string N, double V)>>(p => p
            .Add(c => c.Data, data)
            .Add(c => c.NameField, (Func<(string N, double V), string>)(d => d.N))
            .Add(c => c.ValueField, (Func<(string N, double V), double>)(d => d.V)));

        cut.Markup.Should().NotBeEmpty();
    }

    [Fact]
    public void Chord_AllZeroLinks_DoesNotCrash()
    {
        var nodes = new List<ChordNode>
        {
            new() { Id = "a", Label = "A" },
            new() { Id = "b", Label = "B" },
        };
        var links = new List<ChordLink>
        {
            new() { SourceId = "a", TargetId = "b", Value = 0 },
        };

        var cut = Render<ArcadiaChordChart>(p => p
            .Add(c => c.Data, nodes)
            .Add(c => c.Links, links));

        // SVG renders but zero-value links are skipped — no ribbon paths
        cut.FindAll("path.arcadia-chart__chord-ribbon").Count.Should().Be(0);
    }

    // ── Negative Values ──

    [Fact]
    public void PieChart_NegativeValues_DoesNotCrash()
    {
        var data = new List<(string N, double V)>
        {
            ("A", -10), ("B", 20), ("C", -5)
        };

        var cut = Render<ArcadiaPieChart<(string N, double V)>>(p => p
            .Add(c => c.Data, data)
            .Add(c => c.NameField, (Func<(string N, double V), string>)(d => d.N))
            .Add(c => c.ValueField, (Func<(string N, double V), double>)(d => d.V)));

        cut.Markup.Should().NotBeEmpty();
    }

    // ── Extreme Values ──

    [Fact]
    public void BarChart_ExtremelyLargeValues_DoesNotCrash()
    {
        var data = new List<(string L, double V)>
        {
            ("A", 1e15), ("B", 2e15), ("C", 5e14)
        };
        var series = new List<SeriesConfig<(string L, double V)>>
        {
            new() { Name = "V", Field = d => d.V }
        };

        var cut = Render<ArcadiaBarChart<(string L, double V)>>(p => p
            .Add(c => c.Data, data)
            .Add(c => c.XField, (Func<(string L, double V), object>)(d => d.L))
            .Add(c => c.Series, series));

        cut.Find("svg").Should().NotBeNull();
    }

    [Fact]
    public void LineChart_ExtremelySmallValues_DoesNotCrash()
    {
        var data = new List<(string L, double V)>
        {
            ("A", 1e-15), ("B", 2e-15), ("C", 5e-15)
        };
        var series = new List<SeriesConfig<(string L, double V)>>
        {
            new() { Name = "V", Field = d => d.V }
        };

        var cut = Render<ArcadiaLineChart<(string L, double V)>>(p => p
            .Add(c => c.Data, data)
            .Add(c => c.XField, (Func<(string L, double V), object>)(d => d.L))
            .Add(c => c.Series, series));

        cut.Find("svg").Should().NotBeNull();
    }

    // ── Special Characters ──

    [Fact]
    public void LineChart_SpecialCharLabels_DoesNotCrash()
    {
        var data = new List<(string L, double V)>
        {
            ("<script>", 10), ("A & B", 20), ("\"quotes\"", 30)
        };
        var series = new List<SeriesConfig<(string L, double V)>>
        {
            new() { Name = "V", Field = d => d.V }
        };

        var cut = Render<ArcadiaLineChart<(string L, double V)>>(p => p
            .Add(c => c.Data, data)
            .Add(c => c.XField, (Func<(string L, double V), object>)(d => d.L))
            .Add(c => c.Series, series));

        cut.Markup.Should().NotBeEmpty();
    }
}
