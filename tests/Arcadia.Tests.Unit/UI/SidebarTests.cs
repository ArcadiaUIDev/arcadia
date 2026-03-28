using Bunit;
using FluentAssertions;
using Xunit;
using Arcadia.UI.Components;

namespace Arcadia.Tests.Unit.UI;

public class SidebarTests : ChartTestBase
{
    [Fact]
    public void Visible_True_ShowsSidebar()
    {
        var cut = Render<ArcadiaSidebar>(p => p
            .Add(c => c.Visible, true)
            .AddChildContent("<nav>Menu</nav>"));

        cut.Find(".arcadia-sidebar").Should().NotBeNull();
        cut.Find(".arcadia-sidebar__content").InnerHtml.Should().Contain("Menu");
        cut.Find(".arcadia-sidebar").ClassList.Should().NotContain("arcadia-sidebar--collapsed");
    }

    [Fact]
    public void Visible_False_Collapses()
    {
        var cut = Render<ArcadiaSidebar>(p => p
            .Add(c => c.Visible, false)
            .AddChildContent("<nav>Menu</nav>"));

        cut.Find(".arcadia-sidebar").ClassList.Should().Contain("arcadia-sidebar--collapsed");
    }

    [Fact]
    public void PositionRight_AddsRightClass()
    {
        var cut = Render<ArcadiaSidebar>(p => p
            .Add(c => c.Position, "right")
            .AddChildContent("<nav>Menu</nav>"));

        cut.Find(".arcadia-sidebar").ClassList.Should().Contain("arcadia-sidebar--right");
    }

    [Fact]
    public void PositionLeft_NoRightClass()
    {
        var cut = Render<ArcadiaSidebar>(p => p
            .Add(c => c.Position, "left")
            .AddChildContent("<nav>Menu</nav>"));

        cut.Find(".arcadia-sidebar").ClassList.Should().NotContain("arcadia-sidebar--right");
    }

    [Fact]
    public void ToggleButton_ChangesVisibility()
    {
        var newVisible = true;
        var cut = Render<ArcadiaSidebar>(p => p
            .Add(c => c.Visible, true)
            .Add(c => c.VisibleChanged, v => newVisible = v)
            .AddChildContent("<nav>Menu</nav>"));

        cut.Find(".arcadia-sidebar__toggle").Click();

        newVisible.Should().BeFalse();
    }

    [Fact]
    public void Aria_RoleNavigation()
    {
        var cut = Render<ArcadiaSidebar>(p => p
            .AddChildContent("<nav>Menu</nav>"));

        cut.Find("[role='navigation']").Should().NotBeNull();
    }

    [Fact]
    public void ToggleButton_AriaExpanded()
    {
        var cut = Render<ArcadiaSidebar>(p => p
            .Add(c => c.Visible, true)
            .AddChildContent("<nav>Menu</nav>"));

        cut.Find(".arcadia-sidebar__toggle").GetAttribute("aria-expanded").Should().Be("true");
    }

    [Fact]
    public void Overlay_RendersOverlayDiv_WhenVisible()
    {
        var cut = Render<ArcadiaSidebar>(p => p
            .Add(c => c.Visible, true)
            .Add(c => c.Overlay, true)
            .AddChildContent("<nav>Menu</nav>"));

        cut.Find(".arcadia-sidebar__overlay").Should().NotBeNull();
        cut.Find(".arcadia-sidebar").ClassList.Should().Contain("arcadia-sidebar--overlay");
    }
}
