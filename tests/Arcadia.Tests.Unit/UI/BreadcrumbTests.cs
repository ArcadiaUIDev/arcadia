using Bunit;
using FluentAssertions;
using Xunit;
using Arcadia.UI.Components;

namespace Arcadia.Tests.Unit.UI;

public class BreadcrumbTests : ChartTestBase
{
    [Fact]
    public void Items_RenderInOrder()
    {
        var cut = Render<ArcadiaBreadcrumb>(p => p
            .AddChildContent<ArcadiaBreadcrumbItem>(i => i.Add(x => x.Href, "/").AddChildContent("Home"))
            .AddChildContent<ArcadiaBreadcrumbItem>(i => i.Add(x => x.Href, "/docs").AddChildContent("Docs"))
            .AddChildContent<ArcadiaBreadcrumbItem>(i => i.AddChildContent("Current")));

        var items = cut.FindAll(".arcadia-breadcrumb-item");
        items.Should().HaveCount(3);
        items[0].TextContent.Should().Contain("Home");
        items[1].TextContent.Should().Contain("Docs");
        items[2].TextContent.Should().Contain("Current");
    }

    [Fact]
    public void LastItem_HasAriaCurrent()
    {
        var cut = Render<ArcadiaBreadcrumb>(p => p
            .AddChildContent<ArcadiaBreadcrumbItem>(i => i.Add(x => x.Href, "/").AddChildContent("Home"))
            .AddChildContent<ArcadiaBreadcrumbItem>(i => i.AddChildContent("Current Page")));

        var lastSpan = cut.Find("[aria-current='page']");
        lastSpan.Should().NotBeNull();
        lastSpan.TextContent.Should().Contain("Current Page");
    }

    [Fact]
    public void Href_RendersAnchor()
    {
        var cut = Render<ArcadiaBreadcrumb>(p => p
            .AddChildContent<ArcadiaBreadcrumbItem>(i => i.Add(x => x.Href, "/home").AddChildContent("Home"))
            .AddChildContent<ArcadiaBreadcrumbItem>(i => i.AddChildContent("End")));

        var link = cut.Find(".arcadia-breadcrumb-item__link");
        link.GetAttribute("href").Should().Be("/home");
    }

    [Fact]
    public void NoHref_RendersSpan()
    {
        var cut = Render<ArcadiaBreadcrumb>(p => p
            .AddChildContent<ArcadiaBreadcrumbItem>(i => i.AddChildContent("Current")));

        cut.FindAll("a.arcadia-breadcrumb-item__link").Should().BeEmpty();
        cut.Find(".arcadia-breadcrumb-item__current").Should().NotBeNull();
    }

    [Fact]
    public void Separator_RendersBetweenItems()
    {
        var cut = Render<ArcadiaBreadcrumb>(p => p
            .Add(c => c.Separator, ">")
            .AddChildContent<ArcadiaBreadcrumbItem>(i => i.Add(x => x.Href, "/").AddChildContent("Home"))
            .AddChildContent<ArcadiaBreadcrumbItem>(i => i.AddChildContent("End")));

        var separators = cut.FindAll(".arcadia-breadcrumb-item__separator");
        separators.Should().HaveCount(1);
        separators[0].TextContent.Trim().Should().Be(">");
    }

    [Fact]
    public void Separator_NotAfterLastItem()
    {
        var cut = Render<ArcadiaBreadcrumb>(p => p
            .AddChildContent<ArcadiaBreadcrumbItem>(i => i.Add(x => x.Href, "/").AddChildContent("Home"))
            .AddChildContent<ArcadiaBreadcrumbItem>(i => i.AddChildContent("End")));

        var items = cut.FindAll(".arcadia-breadcrumb-item");
        var lastItem = items[^1];
        lastItem.QuerySelectorAll(".arcadia-breadcrumb-item__separator").Length.Should().Be(0);
    }

    [Fact]
    public void Nav_HasAriaLabel()
    {
        var cut = Render<ArcadiaBreadcrumb>(p => p
            .AddChildContent<ArcadiaBreadcrumbItem>(i => i.AddChildContent("Home")));

        cut.Find("nav").GetAttribute("aria-label").Should().Be("Breadcrumb");
    }

    [Fact]
    public void Separator_HasAriaHidden()
    {
        var cut = Render<ArcadiaBreadcrumb>(p => p
            .AddChildContent<ArcadiaBreadcrumbItem>(i => i.Add(x => x.Href, "/").AddChildContent("Home"))
            .AddChildContent<ArcadiaBreadcrumbItem>(i => i.AddChildContent("End")));

        cut.Find(".arcadia-breadcrumb-item__separator").GetAttribute("aria-hidden").Should().Be("true");
    }
}
