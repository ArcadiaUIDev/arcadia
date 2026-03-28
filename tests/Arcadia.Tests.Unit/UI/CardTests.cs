using Bunit;
using FluentAssertions;
using Xunit;
using Arcadia.UI.Components;

namespace Arcadia.Tests.Unit.UI;

public class CardTests : ChartTestBase
{
    [Fact]
    public void Title_RendersInHeader()
    {
        var cut = Render<ArcadiaCard>(p => p
            .Add(c => c.Title, "Card Title")
            .AddChildContent("<p>Body</p>"));

        cut.Find(".arcadia-card__title").TextContent.Should().Be("Card Title");
    }

    [Fact]
    public void Subtitle_RendersInHeader()
    {
        var cut = Render<ArcadiaCard>(p => p
            .Add(c => c.Title, "Title")
            .Add(c => c.Subtitle, "Subtitle text")
            .AddChildContent("<p>Body</p>"));

        cut.Find(".arcadia-card__subtitle").TextContent.Should().Be("Subtitle text");
    }

    [Fact]
    public void ChildContent_RendersInBody()
    {
        var cut = Render<ArcadiaCard>(p => p
            .AddChildContent("<p>Card body content</p>"));

        cut.Find(".arcadia-card__body").InnerHtml.Should().Contain("Card body content");
    }

    [Fact]
    public void FooterTemplate_RendersInFooter()
    {
        var cut = Render<ArcadiaCard>(p => p
            .AddChildContent("<p>Body</p>")
            .Add(c => c.FooterTemplate, "<button>Action</button>"));

        cut.Find(".arcadia-card__footer").InnerHtml.Should().Contain("Action");
    }

    [Fact]
    public void Clickable_AddsHoverClass()
    {
        var cut = Render<ArcadiaCard>(p => p
            .Add(c => c.Clickable, true)
            .AddChildContent("<p>Body</p>"));

        cut.Find(".arcadia-card").ClassList.Should().Contain("arcadia-card--clickable");
    }

    [Fact]
    public void NotClickable_NoHoverClass()
    {
        var cut = Render<ArcadiaCard>(p => p
            .Add(c => c.Clickable, false)
            .AddChildContent("<p>Body</p>"));

        cut.Find(".arcadia-card").ClassList.Should().NotContain("arcadia-card--clickable");
    }

    [Fact]
    public void OnClick_Fires_WhenClickable()
    {
        var clicked = false;
        var cut = Render<ArcadiaCard>(p => p
            .Add(c => c.Clickable, true)
            .Add(c => c.OnClick, () => clicked = true)
            .AddChildContent("<p>Body</p>"));

        cut.Find(".arcadia-card").Click();

        clicked.Should().BeTrue();
    }

    [Fact]
    public void Clickable_HasButtonRole()
    {
        var cut = Render<ArcadiaCard>(p => p
            .Add(c => c.Clickable, true)
            .AddChildContent("<p>Body</p>"));

        cut.Find(".arcadia-card").GetAttribute("role").Should().Be("button");
    }
}
