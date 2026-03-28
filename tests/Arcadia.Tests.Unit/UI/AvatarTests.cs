using Bunit;
using FluentAssertions;
using Xunit;
using Arcadia.UI.Components;

namespace Arcadia.Tests.Unit.UI;

public class AvatarTests : ChartTestBase
{
    [Theory]
    [InlineData("Alice Chen", "AC")]
    [InlineData("John", "J")]
    [InlineData("Mary Jane Watson", "MW")]
    public void Name_GeneratesInitials(string name, string expected)
    {
        var cut = Render<ArcadiaAvatar>(p => p
            .Add(c => c.Name, name));

        cut.Find(".arcadia-avatar__initials").TextContent.Should().Be(expected);
    }

    [Fact]
    public void Src_RendersImgTag()
    {
        var cut = Render<ArcadiaAvatar>(p => p
            .Add(c => c.Src, "https://example.com/photo.jpg")
            .Add(c => c.Name, "Alice"));

        var img = cut.Find("img.arcadia-avatar__image");
        img.GetAttribute("src").Should().Be("https://example.com/photo.jpg");
        img.GetAttribute("alt").Should().Be("Alice");
    }

    [Fact]
    public void NoSrc_RendersInitials_NotImage()
    {
        var cut = Render<ArcadiaAvatar>(p => p
            .Add(c => c.Name, "Alice Chen"));

        cut.FindAll("img").Should().BeEmpty();
        cut.Find(".arcadia-avatar__initials").Should().NotBeNull();
    }

    [Theory]
    [InlineData("sm", "arcadia-avatar--sm")]
    [InlineData("md", "arcadia-avatar--md")]
    [InlineData("lg", "arcadia-avatar--lg")]
    public void Size_SetsCssClass(string size, string expectedClass)
    {
        var cut = Render<ArcadiaAvatar>(p => p
            .Add(c => c.Name, "A")
            .Add(c => c.Size, size));

        cut.Find(".arcadia-avatar").ClassList.Should().Contain(expectedClass);
    }

    [Theory]
    [InlineData("primary", "arcadia-avatar--primary")]
    [InlineData("success", "arcadia-avatar--success")]
    [InlineData("danger", "arcadia-avatar--danger")]
    public void Color_SetsCssClass(string color, string expectedClass)
    {
        var cut = Render<ArcadiaAvatar>(p => p
            .Add(c => c.Name, "A")
            .Add(c => c.Color, color));

        cut.Find(".arcadia-avatar").ClassList.Should().Contain(expectedClass);
    }

    [Fact]
    public void Aria_RoleImg()
    {
        var cut = Render<ArcadiaAvatar>(p => p
            .Add(c => c.Name, "Alice"));

        cut.Find("[role='img']").Should().NotBeNull();
    }

    [Fact]
    public void Aria_Label_ContainsName()
    {
        var cut = Render<ArcadiaAvatar>(p => p
            .Add(c => c.Name, "Alice Chen"));

        cut.Find("[role='img']").GetAttribute("aria-label").Should().Be("Avatar for Alice Chen");
    }

    [Fact]
    public void NoName_ShowsQuestionMark()
    {
        var cut = Render<ArcadiaAvatar>(p => p.Add(c => c.Name, (string?)null));

        cut.Find(".arcadia-avatar__initials").TextContent.Should().Be("?");
    }
}
