using Bunit;
using FluentAssertions;
using Xunit;
using Arcadia.UI.Components;

namespace Arcadia.Tests.Unit.UI;

public class TooltipTests : ChartTestBase
{
    [Fact]
    public void Text_RendersInPopup()
    {
        var cut = Render<ArcadiaTooltip>(p => p
            .Add(c => c.Text, "Help text")
            .AddChildContent("<span>Hover me</span>"));

        cut.Find(".arcadia-tooltip__text").TextContent.Should().Be("Help text");
    }

    [Theory]
    [InlineData("top", "arcadia-tooltip--top")]
    [InlineData("bottom", "arcadia-tooltip--bottom")]
    [InlineData("left", "arcadia-tooltip--left")]
    [InlineData("right", "arcadia-tooltip--right")]
    public void Position_AppliesCssClass(string position, string expectedClass)
    {
        var cut = Render<ArcadiaTooltip>(p => p
            .Add(c => c.Text, "Tip")
            .Add(c => c.Position, position)
            .AddChildContent("<span>Target</span>"));

        cut.Find(".arcadia-tooltip").ClassList.Should().Contain(expectedClass);
    }

    [Fact]
    public void Aria_RoleTooltip()
    {
        var cut = Render<ArcadiaTooltip>(p => p
            .Add(c => c.Text, "Tip")
            .AddChildContent("<span>Target</span>"));

        cut.Find("[role='tooltip']").Should().NotBeNull();
    }

    [Fact]
    public void Aria_DescribedBy_LinksToPopup()
    {
        var cut = Render<ArcadiaTooltip>(p => p
            .Add(c => c.Text, "Tip")
            .AddChildContent("<span>Target</span>"));

        var wrapper = cut.Find(".arcadia-tooltip");
        var describedBy = wrapper.GetAttribute("aria-describedby");
        describedBy.Should().NotBeNullOrEmpty();

        var popup = cut.Find($"#{describedBy}");
        popup.GetAttribute("role").Should().Be("tooltip");
    }

    [Fact]
    public void ChildContent_Renders()
    {
        var cut = Render<ArcadiaTooltip>(p => p
            .Add(c => c.Text, "Tip")
            .AddChildContent("<button>Click me</button>"));

        cut.Find("button").TextContent.Should().Be("Click me");
    }

    [Fact]
    public void DefaultPosition_IsTop()
    {
        var cut = Render<ArcadiaTooltip>(p => p
            .Add(c => c.Text, "Tip")
            .AddChildContent("<span>X</span>"));

        cut.Find(".arcadia-tooltip").ClassList.Should().Contain("arcadia-tooltip--top");
    }
}
