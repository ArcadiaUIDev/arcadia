using Bunit;
using FluentAssertions;
using Xunit;
using Arcadia.UI.Components;

namespace Arcadia.Tests.Unit.UI;

public class TabsTests : ChartTestBase
{
    [Fact]
    public void Default_RendersFirstTabActive()
    {
        var cut = Render<ArcadiaTabs>(p => p
            .Add(c => c.ActiveIndex, 0)
            .AddChildContent<ArcadiaTabPanel>(tp => tp.Add(t => t.Title, "Tab 1").AddChildContent("Content 1"))
            .AddChildContent<ArcadiaTabPanel>(tp => tp.Add(t => t.Title, "Tab 2").AddChildContent("Content 2")));

        var tabs = cut.FindAll("[role='tab']");
        tabs.Should().HaveCount(2);
        tabs[0].GetAttribute("aria-selected").Should().Be("true");
        tabs[1].GetAttribute("aria-selected").Should().Be("false");
    }

    [Fact]
    public void ActiveIndex_SelectsCorrectTab()
    {
        var cut = Render<ArcadiaTabs>(p => p
            .Add(c => c.ActiveIndex, 1)
            .AddChildContent<ArcadiaTabPanel>(tp => tp.Add(t => t.Title, "Tab 1").AddChildContent("Content 1"))
            .AddChildContent<ArcadiaTabPanel>(tp => tp.Add(t => t.Title, "Tab 2").AddChildContent("Content 2")));

        var tabs = cut.FindAll("[role='tab']");
        tabs[1].GetAttribute("aria-selected").Should().Be("true");

        var panel = cut.Find("[role='tabpanel']");
        panel.TextContent.Should().Contain("Content 2");
    }

    [Fact]
    public void DisabledTab_NotClickable()
    {
        var changed = false;
        var cut = Render<ArcadiaTabs>(p => p
            .Add(c => c.ActiveIndex, 0)
            .Add(c => c.ActiveIndexChanged, _ => changed = true)
            .AddChildContent<ArcadiaTabPanel>(tp => tp.Add(t => t.Title, "Tab 1").AddChildContent("Content 1"))
            .AddChildContent<ArcadiaTabPanel>(tp => tp.Add(t => t.Title, "Tab 2").Add(t => t.Disabled, true).AddChildContent("Content 2")));

        var disabledTab = cut.FindAll("[role='tab']")[1];
        disabledTab.HasAttribute("disabled").Should().BeTrue();
        disabledTab.Click();

        changed.Should().BeFalse();
    }

    [Fact]
    public void ActiveIndexChanged_Fires_OnTabClick()
    {
        var newIndex = -1;
        var cut = Render<ArcadiaTabs>(p => p
            .Add(c => c.ActiveIndex, 0)
            .Add(c => c.ActiveIndexChanged, i => newIndex = i)
            .AddChildContent<ArcadiaTabPanel>(tp => tp.Add(t => t.Title, "Tab 1").AddChildContent("Content 1"))
            .AddChildContent<ArcadiaTabPanel>(tp => tp.Add(t => t.Title, "Tab 2").AddChildContent("Content 2")));

        cut.FindAll("[role='tab']")[1].Click();

        newIndex.Should().Be(1);
    }

    [Fact]
    public void Aria_RoleTablist_OnHeader()
    {
        var cut = Render<ArcadiaTabs>(p => p
            .AddChildContent<ArcadiaTabPanel>(tp => tp.Add(t => t.Title, "Tab 1").AddChildContent("C1")));

        cut.Find("[role='tablist']").Should().NotBeNull();
    }

    [Fact]
    public void Aria_RoleTab_OnButtons()
    {
        var cut = Render<ArcadiaTabs>(p => p
            .AddChildContent<ArcadiaTabPanel>(tp => tp.Add(t => t.Title, "Tab 1").AddChildContent("C1"))
            .AddChildContent<ArcadiaTabPanel>(tp => tp.Add(t => t.Title, "Tab 2").AddChildContent("C2")));

        cut.FindAll("[role='tab']").Should().HaveCount(2);
    }

    [Fact]
    public void Aria_RoleTabpanel_OnContent()
    {
        var cut = Render<ArcadiaTabs>(p => p
            .Add(c => c.ActiveIndex, 0)
            .AddChildContent<ArcadiaTabPanel>(tp => tp.Add(t => t.Title, "Tab 1").AddChildContent("C1")));

        cut.Find("[role='tabpanel']").Should().NotBeNull();
    }
}
