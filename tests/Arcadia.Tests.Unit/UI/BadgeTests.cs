using Bunit;
using FluentAssertions;
using Xunit;
using Arcadia.UI.Components;

namespace Arcadia.Tests.Unit.UI;

public class BadgeTests : ChartTestBase
{
    [Fact]
    public void Content_RendersCount()
    {
        var cut = Render<ArcadiaBadge>(p => p
            .Add(c => c.Content, "5"));

        cut.Find(".arcadia-badge__indicator").TextContent.Trim().Should().Be("5");
    }

    [Fact]
    public void Dot_RendersSmallDot_NoText()
    {
        var cut = Render<ArcadiaBadge>(p => p
            .Add(c => c.Dot, true)
            .Add(c => c.Content, "5"));

        var indicator = cut.Find(".arcadia-badge__indicator");
        indicator.ClassList.Should().Contain("arcadia-badge__indicator--dot");
        indicator.TextContent.Trim().Should().BeEmpty();
    }

    [Theory]
    [InlineData("primary", "arcadia-badge__indicator--primary")]
    [InlineData("success", "arcadia-badge__indicator--success")]
    [InlineData("danger", "arcadia-badge__indicator--danger")]
    [InlineData("warning", "arcadia-badge__indicator--warning")]
    public void Color_SetsCssClass(string color, string expectedClass)
    {
        var cut = Render<ArcadiaBadge>(p => p
            .Add(c => c.Content, "3")
            .Add(c => c.Color, color));

        cut.Find(".arcadia-badge__indicator").ClassList.Should().Contain(expectedClass);
    }

    [Fact]
    public void Aria_RoleStatus()
    {
        var cut = Render<ArcadiaBadge>(p => p
            .Add(c => c.Content, "1"));

        cut.Find("[role='status']").Should().NotBeNull();
    }

    [Fact]
    public void Aria_Label_MatchesContent()
    {
        var cut = Render<ArcadiaBadge>(p => p
            .Add(c => c.Content, "New"));

        cut.Find("[role='status']").GetAttribute("aria-label").Should().Be("New");
    }

    [Fact]
    public void Dot_AriaLabel_IsStatusIndicator()
    {
        var cut = Render<ArcadiaBadge>(p => p
            .Add(c => c.Dot, true));

        cut.Find("[role='status']").GetAttribute("aria-label").Should().Be("Status indicator");
    }

    [Fact]
    public void ChildContent_RendersAnchor()
    {
        var cut = Render<ArcadiaBadge>(p => p
            .Add(c => c.Content, "3")
            .AddChildContent("<span>Inbox</span>"));

        cut.Find(".arcadia-badge__anchor").InnerHtml.Should().Contain("Inbox");
    }
}
