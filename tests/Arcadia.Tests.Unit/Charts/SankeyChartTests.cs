using Bunit;
using FluentAssertions;
using Arcadia.Charts.Components.Charts;
using Arcadia.Charts.Core;
using Xunit;

namespace Arcadia.Tests.Unit.Charts;

public class SankeyChartTests : ChartTestBase
{
    private static readonly List<SankeyNode> BasicNodes = new()
    {
        new() { Id = "a", Label = "Source A" },
        new() { Id = "b", Label = "Source B" },
        new() { Id = "c", Label = "Middle" },
        new() { Id = "d", Label = "End X" },
        new() { Id = "e", Label = "End Y" },
    };

    private static readonly List<SankeyLink> BasicLinks = new()
    {
        new() { SourceId = "a", TargetId = "c", Value = 100 },
        new() { SourceId = "b", TargetId = "c", Value = 50 },
        new() { SourceId = "c", TargetId = "d", Value = 90 },
        new() { SourceId = "c", TargetId = "e", Value = 60 },
    };

    // ── Rendering ───────────────────────────────────────────

    [Fact]
    public void Renders_SvgWithNodesAndLinks()
    {
        var cut = Render<ArcadiaSankeyChart>(p => p
            .Add(c => c.Data, BasicNodes)
            .Add(c => c.Links, BasicLinks)
            .Add(c => c.Width, 600)
            .Add(c => c.Height, 400));

        cut.Find("svg[data-chart]").Should().NotBeNull();
        // 5 nodes → 5 rects
        cut.FindAll("rect.arcadia-chart__sankey-node").Count.Should().Be(5);
        // 4 links → 4 paths
        cut.FindAll("path.arcadia-chart__sankey-link").Count.Should().Be(4);
    }

    [Fact]
    public void Renders_Title()
    {
        var cut = Render<ArcadiaSankeyChart>(p => p
            .Add(c => c.Data, BasicNodes)
            .Add(c => c.Links, BasicLinks)
            .Add(c => c.Title, "Flow Diagram"));

        cut.Find(".arcadia-chart__title").TextContent.Should().Be("Flow Diagram");
    }

    [Fact]
    public void Renders_Subtitle_WhenTitlePresent()
    {
        var cut = Render<ArcadiaSankeyChart>(p => p
            .Add(c => c.Data, BasicNodes)
            .Add(c => c.Links, BasicLinks)
            .Add(c => c.Title, "Flow")
            .Add(c => c.Subtitle, "FY2026"));

        cut.Markup.Should().Contain("FY2026");
        cut.Find(".arcadia-chart__subtitle").Should().NotBeNull();
    }

    [Fact]
    public void DoesNotRender_Subtitle_WithoutTitle()
    {
        var cut = Render<ArcadiaSankeyChart>(p => p
            .Add(c => c.Data, BasicNodes)
            .Add(c => c.Links, BasicLinks)
            .Add(c => c.Subtitle, "FY2026"));

        cut.Markup.Should().NotContain("FY2026");
    }

    [Fact]
    public void Renders_WithCustomDimensions()
    {
        var cut = Render<ArcadiaSankeyChart>(p => p
            .Add(c => c.Data, BasicNodes)
            .Add(c => c.Links, BasicLinks)
            .Add(c => c.Width, 900)
            .Add(c => c.Height, 500));

        var svg = cut.Find("svg[data-chart]");
        svg.GetAttribute("width").Should().Be("900");
        svg.GetAttribute("height").Should().Be("500");
    }

    // ── Labels ──────────────────────────────────────────────

    [Fact]
    public void Renders_NodeLabels_WhenShowLabelsTrue()
    {
        var cut = Render<ArcadiaSankeyChart>(p => p
            .Add(c => c.Data, BasicNodes)
            .Add(c => c.Links, BasicLinks)
            .Add(c => c.ShowLabels, true));

        cut.Markup.Should().Contain("Source A");
        cut.Markup.Should().Contain("End Y");
    }

    [Fact]
    public void HidesLabels_WhenShowLabelsFalse()
    {
        var cut = Render<ArcadiaSankeyChart>(p => p
            .Add(c => c.Data, BasicNodes)
            .Add(c => c.Links, BasicLinks)
            .Add(c => c.ShowLabels, false));

        cut.FindAll(".arcadia-chart__data-label").Count.Should().Be(0);
    }

    // ── Animation ───────────────────────────────────────────

    [Fact]
    public void Applies_AnimationClass_WhenAnimateOnLoadTrue()
    {
        var cut = Render<ArcadiaSankeyChart>(p => p
            .Add(c => c.Data, BasicNodes)
            .Add(c => c.Links, BasicLinks)
            .Add(c => c.AnimateOnLoad, true));

        cut.FindAll(".arcadia-animate-sankey").Count.Should().Be(4);
    }

    [Fact]
    public void NoAnimationClass_WhenAnimateOnLoadFalse()
    {
        var cut = Render<ArcadiaSankeyChart>(p => p
            .Add(c => c.Data, BasicNodes)
            .Add(c => c.Links, BasicLinks)
            .Add(c => c.AnimateOnLoad, false));

        cut.FindAll(".arcadia-animate-sankey").Count.Should().Be(0);
    }

    // ── Accessibility ───────────────────────────────────────

    [Fact]
    public void Renders_AriaLabel()
    {
        var cut = Render<ArcadiaSankeyChart>(p => p
            .Add(c => c.Data, BasicNodes)
            .Add(c => c.Links, BasicLinks)
            .Add(c => c.AriaLabel, "Energy flow diagram"));

        cut.Find("svg[data-chart]").GetAttribute("aria-label").Should().Be("Energy flow diagram");
    }

    [Fact]
    public void Renders_DefaultAriaLabel_FromTitle()
    {
        var cut = Render<ArcadiaSankeyChart>(p => p
            .Add(c => c.Data, BasicNodes)
            .Add(c => c.Links, BasicLinks)
            .Add(c => c.Title, "Budget Flow"));

        cut.Find("svg[data-chart]").GetAttribute("aria-label").Should().Be("Budget Flow");
    }

    [Fact]
    public void Renders_FallbackAriaLabel_WhenNoTitleOrLabel()
    {
        var cut = Render<ArcadiaSankeyChart>(p => p
            .Add(c => c.Data, BasicNodes)
            .Add(c => c.Links, BasicLinks));

        cut.Find("svg[data-chart]").GetAttribute("aria-label").Should().Be("Sankey diagram");
    }

    [Fact]
    public void Renders_ScreenReaderTable()
    {
        var cut = Render<ArcadiaSankeyChart>(p => p
            .Add(c => c.Data, BasicNodes)
            .Add(c => c.Links, BasicLinks));

        var table = cut.Find("table.arcadia-sr-only");
        table.Should().NotBeNull();
        table.GetAttribute("aria-label").Should().Be("Sankey diagram data");

        // 4 links → 4 data rows
        cut.FindAll("table.arcadia-sr-only tbody tr").Count.Should().Be(4);

        // Verify headers
        var headers = cut.FindAll("table.arcadia-sr-only thead th");
        headers[0].TextContent.Should().Be("Source");
        headers[1].TextContent.Should().Be("Target");
        headers[2].TextContent.Should().Be("Value");
    }

    [Fact]
    public void ScreenReaderTable_ContainsCorrectData()
    {
        var cut = Render<ArcadiaSankeyChart>(p => p
            .Add(c => c.Data, BasicNodes)
            .Add(c => c.Links, BasicLinks));

        var rows = cut.FindAll("table.arcadia-sr-only tbody tr");
        rows[0].TextContent.Should().Contain("Source A");
        rows[0].TextContent.Should().Contain("Middle");
        rows[0].TextContent.Should().Contain("100");
    }

    // ── No data / empty states ──────────────────────────────

    [Fact]
    public void NoData_RendersNoDataState()
    {
        var cut = Render<ArcadiaSankeyChart>(p => p
            .Add(c => c.Data, new List<SankeyNode>())
            .Add(c => c.Links, BasicLinks));

        cut.FindAll("svg").Count.Should().Be(0);
        cut.Markup.Should().Contain("No data available");
    }

    [Fact]
    public void NullData_RendersNoDataState()
    {
        var cut = Render<ArcadiaSankeyChart>(p => p
            .Add(c => c.Links, BasicLinks));

        cut.FindAll("svg").Count.Should().Be(0);
    }

    [Fact]
    public void NoLinks_RendersNoDataState()
    {
        var cut = Render<ArcadiaSankeyChart>(p => p
            .Add(c => c.Data, BasicNodes)
            .Add(c => c.Links, new List<SankeyLink>()));

        cut.FindAll("svg").Count.Should().Be(0);
    }

    [Fact]
    public void NullLinks_RendersNoDataState()
    {
        var cut = Render<ArcadiaSankeyChart>(p => p
            .Add(c => c.Data, BasicNodes));

        cut.FindAll("svg").Count.Should().Be(0);
    }

    [Fact]
    public void CustomNoDataMessage()
    {
        var cut = Render<ArcadiaSankeyChart>(p => p
            .Add(c => c.Data, new List<SankeyNode>())
            .Add(c => c.Links, new List<SankeyLink>())
            .Add(c => c.NoDataMessage, "Please add some data"));

        cut.Markup.Should().Contain("Please add some data");
    }

    // ── Edge cases ──────────────────────────────────────────

    [Fact]
    public void SkipsLinks_WithNegativeValues()
    {
        var links = new List<SankeyLink>
        {
            new() { SourceId = "a", TargetId = "c", Value = 100 },
            new() { SourceId = "b", TargetId = "c", Value = -50 }, // should be skipped
        };

        var cut = Render<ArcadiaSankeyChart>(p => p
            .Add(c => c.Data, BasicNodes)
            .Add(c => c.Links, links));

        cut.FindAll("path.arcadia-chart__sankey-link").Count.Should().Be(1);
    }

    [Fact]
    public void SkipsLinks_WithZeroValues()
    {
        var links = new List<SankeyLink>
        {
            new() { SourceId = "a", TargetId = "c", Value = 100 },
            new() { SourceId = "b", TargetId = "c", Value = 0 }, // should be skipped
        };

        var cut = Render<ArcadiaSankeyChart>(p => p
            .Add(c => c.Data, BasicNodes)
            .Add(c => c.Links, links));

        cut.FindAll("path.arcadia-chart__sankey-link").Count.Should().Be(1);
    }

    [Fact]
    public void SkipsSelfLinks()
    {
        var links = new List<SankeyLink>
        {
            new() { SourceId = "a", TargetId = "c", Value = 100 },
            new() { SourceId = "c", TargetId = "c", Value = 50 }, // self-link, should be skipped
        };

        var cut = Render<ArcadiaSankeyChart>(p => p
            .Add(c => c.Data, BasicNodes)
            .Add(c => c.Links, links));

        cut.FindAll("path.arcadia-chart__sankey-link").Count.Should().Be(1);
    }

    [Fact]
    public void SkipsLinks_WithInvalidNodeIds()
    {
        var links = new List<SankeyLink>
        {
            new() { SourceId = "a", TargetId = "c", Value = 100 },
            new() { SourceId = "nonexistent", TargetId = "c", Value = 50 },
            new() { SourceId = "a", TargetId = "missing", Value = 30 },
        };

        var cut = Render<ArcadiaSankeyChart>(p => p
            .Add(c => c.Data, BasicNodes)
            .Add(c => c.Links, links));

        cut.FindAll("path.arcadia-chart__sankey-link").Count.Should().Be(1);
    }

    [Fact]
    public void HandlesCyclicLinks_Gracefully()
    {
        var nodes = new List<SankeyNode>
        {
            new() { Id = "x", Label = "X" },
            new() { Id = "y", Label = "Y" },
            new() { Id = "z", Label = "Z" },
        };
        var links = new List<SankeyLink>
        {
            new() { SourceId = "x", TargetId = "y", Value = 100 },
            new() { SourceId = "y", TargetId = "z", Value = 80 },
            new() { SourceId = "z", TargetId = "x", Value = 60 }, // creates cycle
        };

        // Should not throw — renders what it can
        var cut = Render<ArcadiaSankeyChart>(p => p
            .Add(c => c.Data, nodes)
            .Add(c => c.Links, links));

        cut.Find("svg[data-chart]").Should().NotBeNull();
        cut.FindAll("rect.arcadia-chart__sankey-node").Count.Should().Be(3);
    }

    [Fact]
    public void Renders_SingleNode_NoLinks()
    {
        var nodes = new List<SankeyNode>
        {
            new() { Id = "solo", Label = "Solo" },
        };
        // HasData requires Links.Count > 0 — so this shows no-data
        var cut = Render<ArcadiaSankeyChart>(p => p
            .Add(c => c.Data, nodes)
            .Add(c => c.Links, new List<SankeyLink>()));

        cut.FindAll("svg").Count.Should().Be(0);
    }

    [Fact]
    public void Handles_DuplicateNodeIds()
    {
        var nodes = new List<SankeyNode>
        {
            new() { Id = "a", Label = "First A" },
            new() { Id = "a", Label = "Duplicate A" }, // duplicate
            new() { Id = "b", Label = "B" },
        };
        var links = new List<SankeyLink>
        {
            new() { SourceId = "a", TargetId = "b", Value = 100 },
        };

        var cut = Render<ArcadiaSankeyChart>(p => p
            .Add(c => c.Data, nodes)
            .Add(c => c.Links, links));

        // Should use first "a" and skip duplicate
        cut.FindAll("rect.arcadia-chart__sankey-node").Count.Should().Be(2);
        cut.Markup.Should().Contain("First A");
    }

    // ── Multi-level layout ──────────────────────────────────

    [Fact]
    public void Renders_FourLevelFlow()
    {
        var nodes = new List<SankeyNode>
        {
            new() { Id = "s1", Label = "Source 1" },
            new() { Id = "s2", Label = "Source 2" },
            new() { Id = "m1", Label = "Mid 1" },
            new() { Id = "m2", Label = "Mid 2" },
            new() { Id = "e1", Label = "End 1" },
            new() { Id = "e2", Label = "End 2" },
            new() { Id = "f1", Label = "Final 1" },
        };
        var links = new List<SankeyLink>
        {
            new() { SourceId = "s1", TargetId = "m1", Value = 100 },
            new() { SourceId = "s2", TargetId = "m2", Value = 80 },
            new() { SourceId = "m1", TargetId = "e1", Value = 60 },
            new() { SourceId = "m1", TargetId = "e2", Value = 40 },
            new() { SourceId = "m2", TargetId = "e2", Value = 80 },
            new() { SourceId = "e1", TargetId = "f1", Value = 60 },
            new() { SourceId = "e2", TargetId = "f1", Value = 120 },
        };

        var cut = Render<ArcadiaSankeyChart>(p => p
            .Add(c => c.Data, nodes)
            .Add(c => c.Links, links));

        cut.FindAll("rect.arcadia-chart__sankey-node").Count.Should().Be(7);
        cut.FindAll("path.arcadia-chart__sankey-link").Count.Should().Be(7);
    }

    // ── Palette / colors ────────────────────────────────────

    [Fact]
    public void Applies_CustomPalette()
    {
        var cut = Render<ArcadiaSankeyChart>(p => p
            .Add(c => c.Data, BasicNodes)
            .Add(c => c.Links, BasicLinks)
            .Add(c => c.Palette, ChartPalette.Pastel));

        // Should render without error — palette colors applied to nodes
        cut.FindAll("rect.arcadia-chart__sankey-node").Count.Should().Be(5);
    }

    [Fact]
    public void Applies_NodeColorOverride()
    {
        var nodes = new List<SankeyNode>
        {
            new() { Id = "a", Label = "A", Color = "#ff0000" },
            new() { Id = "b", Label = "B" },
        };
        var links = new List<SankeyLink>
        {
            new() { SourceId = "a", TargetId = "b", Value = 100 },
        };

        var cut = Render<ArcadiaSankeyChart>(p => p
            .Add(c => c.Data, nodes)
            .Add(c => c.Links, links));

        var rects = cut.FindAll("rect.arcadia-chart__sankey-node");
        rects[0].GetAttribute("fill").Should().Be("#ff0000");
    }

    [Fact]
    public void Applies_LinkColorOverride()
    {
        var links = new List<SankeyLink>
        {
            new() { SourceId = "a", TargetId = "c", Value = 100, Color = "#00ff00" },
            new() { SourceId = "b", TargetId = "c", Value = 50 },
        };

        var cut = Render<ArcadiaSankeyChart>(p => p
            .Add(c => c.Data, BasicNodes)
            .Add(c => c.Links, links));

        var paths = cut.FindAll("path.arcadia-chart__sankey-link");
        paths[0].GetAttribute("fill").Should().Be("#00ff00");
    }

    // ── NodeWidth / NodePadding ─────────────────────────────

    [Fact]
    public void Applies_CustomNodeWidth()
    {
        var cut = Render<ArcadiaSankeyChart>(p => p
            .Add(c => c.Data, BasicNodes)
            .Add(c => c.Links, BasicLinks)
            .Add(c => c.NodeWidth, 30));

        var rects = cut.FindAll("rect.arcadia-chart__sankey-node");
        rects[0].GetAttribute("width").Should().Be("30.0");
    }

    // ── Export toolbar ──────────────────────────────────────

    [Fact]
    public void ShowsToolbar_WhenEnabled()
    {
        var cut = Render<ArcadiaSankeyChart>(p => p
            .Add(c => c.Data, BasicNodes)
            .Add(c => c.Links, BasicLinks)
            .Add(c => c.ShowToolbar, true));

        cut.FindAll(".arcadia-chart-toolbar").Count.Should().Be(1);
    }

    [Fact]
    public void HidesToolbar_WhenDisabled()
    {
        var cut = Render<ArcadiaSankeyChart>(p => p
            .Add(c => c.Data, BasicNodes)
            .Add(c => c.Links, BasicLinks)
            .Add(c => c.ShowToolbar, false));

        cut.FindAll(".arcadia-chart-toolbar").Count.Should().Be(0);
    }

    // ── Loading state ───────────────────────────────────────

    [Fact]
    public void Applies_LoadingClass()
    {
        var cut = Render<ArcadiaSankeyChart>(p => p
            .Add(c => c.Data, BasicNodes)
            .Add(c => c.Links, BasicLinks)
            .Add(c => c.Loading, true));

        cut.Find(".arcadia-chart--loading").Should().NotBeNull();
    }

    // ── Container ───────────────────────────────────────────

    [Fact]
    public void Passes_AdditionalAttributes()
    {
        var cut = Render<ArcadiaSankeyChart>(p => p
            .Add(c => c.Data, BasicNodes)
            .Add(c => c.Links, BasicLinks)
            .AddUnmatched("data-testid", "my-sankey"));

        cut.Find("[data-testid='my-sankey']").Should().NotBeNull();
    }
}
