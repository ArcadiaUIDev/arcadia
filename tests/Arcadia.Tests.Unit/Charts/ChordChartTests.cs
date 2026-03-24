using Bunit;
using FluentAssertions;
using Arcadia.Charts.Components.Charts;
using Arcadia.Charts.Core;
using Xunit;

namespace Arcadia.Tests.Unit.Charts;

public class ChordChartTests : ChartTestBase
{
    private static readonly List<ChordNode> BasicNodes = new()
    {
        new() { Id = "a", Label = "Alpha" },
        new() { Id = "b", Label = "Beta" },
        new() { Id = "c", Label = "Gamma" },
        new() { Id = "d", Label = "Delta" },
    };

    private static readonly List<ChordLink> BasicLinks = new()
    {
        new() { SourceId = "a", TargetId = "b", Value = 100 },
        new() { SourceId = "a", TargetId = "c", Value = 60 },
        new() { SourceId = "b", TargetId = "c", Value = 80 },
        new() { SourceId = "b", TargetId = "d", Value = 40 },
        new() { SourceId = "c", TargetId = "d", Value = 50 },
    };

    // ── Rendering ───────────────────────────────────────────

    [Fact]
    public void Renders_SvgWithArcsAndChords()
    {
        var cut = Render<ArcadiaChordChart>(p => p
            .Add(c => c.Data, BasicNodes)
            .Add(c => c.Links, BasicLinks)
            .Add(c => c.Width, 500)
            .Add(c => c.Height, 500));

        cut.Find("svg[data-chart]").Should().NotBeNull();
        // 4 nodes → 4 outer ring arcs
        cut.FindAll("path.arcadia-chart__chord-arc").Count.Should().Be(4);
        // 5 links → 5 chord ribbons
        cut.FindAll("path.arcadia-chart__chord-ribbon").Count.Should().Be(5);
    }

    [Fact]
    public void Renders_Title()
    {
        var cut = Render<ArcadiaChordChart>(p => p
            .Add(c => c.Data, BasicNodes)
            .Add(c => c.Links, BasicLinks)
            .Add(c => c.Title, "Connections"));

        cut.Find(".arcadia-chart__title").TextContent.Should().Be("Connections");
    }

    [Fact]
    public void Renders_Subtitle_WhenTitlePresent()
    {
        var cut = Render<ArcadiaChordChart>(p => p
            .Add(c => c.Data, BasicNodes)
            .Add(c => c.Links, BasicLinks)
            .Add(c => c.Title, "Network")
            .Add(c => c.Subtitle, "Q1 2026"));

        cut.Markup.Should().Contain("Q1 2026");
        cut.Find(".arcadia-chart__subtitle").Should().NotBeNull();
    }

    [Fact]
    public void DoesNotRender_Subtitle_WithoutTitle()
    {
        var cut = Render<ArcadiaChordChart>(p => p
            .Add(c => c.Data, BasicNodes)
            .Add(c => c.Links, BasicLinks)
            .Add(c => c.Subtitle, "Q1 2026"));

        cut.Markup.Should().NotContain("Q1 2026");
    }

    [Fact]
    public void Renders_WithCustomDimensions()
    {
        var cut = Render<ArcadiaChordChart>(p => p
            .Add(c => c.Data, BasicNodes)
            .Add(c => c.Links, BasicLinks)
            .Add(c => c.Width, 700)
            .Add(c => c.Height, 700));

        var svg = cut.Find("svg[data-chart]");
        svg.GetAttribute("width").Should().Be("700");
        svg.GetAttribute("height").Should().Be("700");
    }

    // ── Labels ──────────────────────────────────────────────

    [Fact]
    public void Renders_Labels_WhenShowLabelsTrue()
    {
        var cut = Render<ArcadiaChordChart>(p => p
            .Add(c => c.Data, BasicNodes)
            .Add(c => c.Links, BasicLinks)
            .Add(c => c.ShowLabels, true));

        cut.Markup.Should().Contain("Alpha");
        cut.Markup.Should().Contain("Delta");
        cut.FindAll(".arcadia-chart__chord-label").Count.Should().Be(4);
    }

    [Fact]
    public void HidesLabels_WhenShowLabelsFalse()
    {
        var cut = Render<ArcadiaChordChart>(p => p
            .Add(c => c.Data, BasicNodes)
            .Add(c => c.Links, BasicLinks)
            .Add(c => c.ShowLabels, false));

        cut.FindAll(".arcadia-chart__chord-label").Count.Should().Be(0);
    }

    // ── Animation ───────────────────────────────────────────

    [Fact]
    public void Applies_AnimationClass_WhenAnimateOnLoadTrue()
    {
        var cut = Render<ArcadiaChordChart>(p => p
            .Add(c => c.Data, BasicNodes)
            .Add(c => c.Links, BasicLinks)
            .Add(c => c.AnimateOnLoad, true));

        cut.FindAll(".arcadia-animate-chord").Count.Should().Be(5);
    }

    [Fact]
    public void NoAnimationClass_WhenAnimateOnLoadFalse()
    {
        var cut = Render<ArcadiaChordChart>(p => p
            .Add(c => c.Data, BasicNodes)
            .Add(c => c.Links, BasicLinks)
            .Add(c => c.AnimateOnLoad, false));

        cut.FindAll(".arcadia-animate-chord").Count.Should().Be(0);
    }

    // ── Accessibility ───────────────────────────────────────

    [Fact]
    public void Renders_AriaLabel()
    {
        var cut = Render<ArcadiaChordChart>(p => p
            .Add(c => c.Data, BasicNodes)
            .Add(c => c.Links, BasicLinks)
            .Add(c => c.AriaLabel, "Trade relationships"));

        cut.Find("svg[data-chart]").GetAttribute("aria-label").Should().Be("Trade relationships");
    }

    [Fact]
    public void Renders_DefaultAriaLabel_FromTitle()
    {
        var cut = Render<ArcadiaChordChart>(p => p
            .Add(c => c.Data, BasicNodes)
            .Add(c => c.Links, BasicLinks)
            .Add(c => c.Title, "Trade Flow"));

        cut.Find("svg[data-chart]").GetAttribute("aria-label").Should().Be("Trade Flow");
    }

    [Fact]
    public void Renders_FallbackAriaLabel_WhenNoTitleOrLabel()
    {
        var cut = Render<ArcadiaChordChart>(p => p
            .Add(c => c.Data, BasicNodes)
            .Add(c => c.Links, BasicLinks));

        cut.Find("svg[data-chart]").GetAttribute("aria-label").Should().Be("Chord diagram");
    }

    [Fact]
    public void Renders_ScreenReaderTable()
    {
        var cut = Render<ArcadiaChordChart>(p => p
            .Add(c => c.Data, BasicNodes)
            .Add(c => c.Links, BasicLinks));

        var table = cut.Find("table.arcadia-sr-only");
        table.Should().NotBeNull();
        table.GetAttribute("aria-label").Should().Be("Chord diagram data");
        cut.FindAll("table.arcadia-sr-only tbody tr").Count.Should().Be(5);

        var headers = cut.FindAll("table.arcadia-sr-only thead th");
        headers[0].TextContent.Should().Be("Source");
        headers[1].TextContent.Should().Be("Target");
        headers[2].TextContent.Should().Be("Value");
    }

    // ── No data / empty states ──────────────────────────────

    [Fact]
    public void NoData_RendersNoDataState()
    {
        var cut = Render<ArcadiaChordChart>(p => p
            .Add(c => c.Data, new List<ChordNode>())
            .Add(c => c.Links, BasicLinks));

        cut.FindAll("svg").Count.Should().Be(0);
        cut.Markup.Should().Contain("No data available");
    }

    [Fact]
    public void NullData_RendersNoDataState()
    {
        var cut = Render<ArcadiaChordChart>(p => p
            .Add(c => c.Links, BasicLinks));

        cut.FindAll("svg").Count.Should().Be(0);
    }

    [Fact]
    public void NoLinks_RendersNoDataState()
    {
        var cut = Render<ArcadiaChordChart>(p => p
            .Add(c => c.Data, BasicNodes)
            .Add(c => c.Links, new List<ChordLink>()));

        cut.FindAll("svg").Count.Should().Be(0);
    }

    [Fact]
    public void CustomNoDataMessage()
    {
        var cut = Render<ArcadiaChordChart>(p => p
            .Add(c => c.Data, new List<ChordNode>())
            .Add(c => c.Links, new List<ChordLink>())
            .Add(c => c.NoDataMessage, "No connections yet"));

        cut.Markup.Should().Contain("No connections yet");
    }

    // ── Edge cases ──────────────────────────────────────────

    [Fact]
    public void SkipsLinks_WithNegativeValues()
    {
        var links = new List<ChordLink>
        {
            new() { SourceId = "a", TargetId = "b", Value = 100 },
            new() { SourceId = "a", TargetId = "c", Value = -50 },
        };

        var cut = Render<ArcadiaChordChart>(p => p
            .Add(c => c.Data, BasicNodes)
            .Add(c => c.Links, links));

        cut.FindAll("path.arcadia-chart__chord-ribbon").Count.Should().Be(1);
    }

    [Fact]
    public void SkipsSelfLinks()
    {
        var links = new List<ChordLink>
        {
            new() { SourceId = "a", TargetId = "b", Value = 100 },
            new() { SourceId = "a", TargetId = "a", Value = 50 },
        };

        var cut = Render<ArcadiaChordChart>(p => p
            .Add(c => c.Data, BasicNodes)
            .Add(c => c.Links, links));

        cut.FindAll("path.arcadia-chart__chord-ribbon").Count.Should().Be(1);
    }

    [Fact]
    public void SkipsLinks_WithInvalidNodeIds()
    {
        var links = new List<ChordLink>
        {
            new() { SourceId = "a", TargetId = "b", Value = 100 },
            new() { SourceId = "missing", TargetId = "b", Value = 50 },
        };

        var cut = Render<ArcadiaChordChart>(p => p
            .Add(c => c.Data, BasicNodes)
            .Add(c => c.Links, links));

        cut.FindAll("path.arcadia-chart__chord-ribbon").Count.Should().Be(1);
    }

    [Fact]
    public void Handles_DuplicateNodeIds()
    {
        var nodes = new List<ChordNode>
        {
            new() { Id = "a", Label = "First" },
            new() { Id = "a", Label = "Duplicate" },
            new() { Id = "b", Label = "B" },
        };
        var links = new List<ChordLink>
        {
            new() { SourceId = "a", TargetId = "b", Value = 100 },
        };

        var cut = Render<ArcadiaChordChart>(p => p
            .Add(c => c.Data, nodes)
            .Add(c => c.Links, links));

        cut.FindAll("path.arcadia-chart__chord-arc").Count.Should().Be(2);
        cut.Markup.Should().Contain("First");
    }

    // ── Colors ──────────────────────────────────────────────

    [Fact]
    public void Applies_CustomPalette()
    {
        var cut = Render<ArcadiaChordChart>(p => p
            .Add(c => c.Data, BasicNodes)
            .Add(c => c.Links, BasicLinks)
            .Add(c => c.Palette, ChartPalette.Pastel));

        cut.FindAll("path.arcadia-chart__chord-arc").Count.Should().Be(4);
    }

    [Fact]
    public void Applies_NodeColorOverride()
    {
        var nodes = new List<ChordNode>
        {
            new() { Id = "a", Label = "A", Color = "#ff0000" },
            new() { Id = "b", Label = "B" },
        };
        var links = new List<ChordLink>
        {
            new() { SourceId = "a", TargetId = "b", Value = 100 },
        };

        var cut = Render<ArcadiaChordChart>(p => p
            .Add(c => c.Data, nodes)
            .Add(c => c.Links, links));

        var arcs = cut.FindAll("path.arcadia-chart__chord-arc");
        arcs[0].GetAttribute("fill").Should().Be("#ff0000");
    }

    [Fact]
    public void Applies_LinkColorOverride()
    {
        var links = new List<ChordLink>
        {
            new() { SourceId = "a", TargetId = "b", Value = 100, Color = "#00ff00" },
        };

        var cut = Render<ArcadiaChordChart>(p => p
            .Add(c => c.Data, BasicNodes)
            .Add(c => c.Links, links));

        var ribbons = cut.FindAll("path.arcadia-chart__chord-ribbon");
        ribbons[0].GetAttribute("fill").Should().Be("#00ff00");
    }

    // ── Toolbar / Loading ───────────────────────────────────

    [Fact]
    public void ShowsToolbar_WhenEnabled()
    {
        var cut = Render<ArcadiaChordChart>(p => p
            .Add(c => c.Data, BasicNodes)
            .Add(c => c.Links, BasicLinks)
            .Add(c => c.ShowToolbar, true));

        cut.FindAll(".arcadia-chart-toolbar").Count.Should().Be(1);
    }

    [Fact]
    public void HidesToolbar_WhenDisabled()
    {
        var cut = Render<ArcadiaChordChart>(p => p
            .Add(c => c.Data, BasicNodes)
            .Add(c => c.Links, BasicLinks)
            .Add(c => c.ShowToolbar, false));

        cut.FindAll(".arcadia-chart-toolbar").Count.Should().Be(0);
    }

    [Fact]
    public void Applies_LoadingClass()
    {
        var cut = Render<ArcadiaChordChart>(p => p
            .Add(c => c.Data, BasicNodes)
            .Add(c => c.Links, BasicLinks)
            .Add(c => c.Loading, true));

        cut.Find(".arcadia-chart--loading").Should().NotBeNull();
    }

    [Fact]
    public void Passes_AdditionalAttributes()
    {
        var cut = Render<ArcadiaChordChart>(p => p
            .Add(c => c.Data, BasicNodes)
            .Add(c => c.Links, BasicLinks)
            .AddUnmatched("data-testid", "my-chord"));

        cut.Find("[data-testid='my-chord']").Should().NotBeNull();
    }
}
