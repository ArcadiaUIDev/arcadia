using Bunit;
using FluentAssertions;
using Xunit;
using Arcadia.UI.Components;

namespace Arcadia.Tests.Unit.UI;

public class AccordionTests : ChartTestBase
{
    [Fact]
    public void Items_RenderCollapsed_ByDefault()
    {
        var cut = Render<ArcadiaAccordion>(p => p
            .AddChildContent<ArcadiaAccordionItem>(i => i.Add(x => x.Title, "Section 1").AddChildContent("Content 1"))
            .AddChildContent<ArcadiaAccordionItem>(i => i.Add(x => x.Title, "Section 2").AddChildContent("Content 2")));

        var triggers = cut.FindAll(".arcadia-accordion-item__trigger");
        triggers.Should().HaveCount(2);

        foreach (var trigger in triggers)
        {
            trigger.GetAttribute("aria-expanded").Should().Be("false");
        }
    }

    [Fact]
    public void Click_ExpandsItem()
    {
        var cut = Render<ArcadiaAccordion>(p => p
            .AddChildContent<ArcadiaAccordionItem>(i => i.Add(x => x.Title, "Section 1").AddChildContent("Content 1")));

        var trigger = cut.Find(".arcadia-accordion-item__trigger");
        trigger.GetAttribute("aria-expanded").Should().Be("false");

        trigger.Click();

        trigger = cut.Find(".arcadia-accordion-item__trigger");
        trigger.GetAttribute("aria-expanded").Should().Be("true");
    }

    [Fact]
    public void MultipleFalse_CollapsesSiblings()
    {
        var cut = Render<ArcadiaAccordion>(p => p
            .Add(c => c.Multiple, false)
            .AddChildContent<ArcadiaAccordionItem>(i => i.Add(x => x.Title, "S1").AddChildContent("C1"))
            .AddChildContent<ArcadiaAccordionItem>(i => i.Add(x => x.Title, "S2").AddChildContent("C2")));

        var triggers = cut.FindAll(".arcadia-accordion-item__trigger");

        // Expand first item
        triggers[0].Click();
        cut.FindAll(".arcadia-accordion-item__trigger")[0].GetAttribute("aria-expanded").Should().Be("true");

        // Expand second item - first should collapse
        cut.FindAll(".arcadia-accordion-item__trigger")[1].Click();
        cut.FindAll(".arcadia-accordion-item__trigger")[0].GetAttribute("aria-expanded").Should().Be("false");
        cut.FindAll(".arcadia-accordion-item__trigger")[1].GetAttribute("aria-expanded").Should().Be("true");
    }

    [Fact]
    public void MultipleTrue_KeepsSiblingsOpen()
    {
        var cut = Render<ArcadiaAccordion>(p => p
            .Add(c => c.Multiple, true)
            .AddChildContent<ArcadiaAccordionItem>(i => i.Add(x => x.Title, "S1").AddChildContent("C1"))
            .AddChildContent<ArcadiaAccordionItem>(i => i.Add(x => x.Title, "S2").AddChildContent("C2")));

        cut.FindAll(".arcadia-accordion-item__trigger")[0].Click();
        cut.FindAll(".arcadia-accordion-item__trigger")[1].Click();

        cut.FindAll(".arcadia-accordion-item__trigger")[0].GetAttribute("aria-expanded").Should().Be("true");
        cut.FindAll(".arcadia-accordion-item__trigger")[1].GetAttribute("aria-expanded").Should().Be("true");
    }

    [Fact]
    public void Aria_ExpandedAttribute_Toggles()
    {
        var cut = Render<ArcadiaAccordion>(p => p
            .AddChildContent<ArcadiaAccordionItem>(i => i.Add(x => x.Title, "S1").AddChildContent("C1")));

        var trigger = cut.Find(".arcadia-accordion-item__trigger");
        trigger.GetAttribute("aria-expanded").Should().Be("false");

        trigger.Click();
        cut.Find(".arcadia-accordion-item__trigger").GetAttribute("aria-expanded").Should().Be("true");

        cut.Find(".arcadia-accordion-item__trigger").Click();
        cut.Find(".arcadia-accordion-item__trigger").GetAttribute("aria-expanded").Should().Be("false");
    }

    [Fact]
    public void Title_RendersInTrigger()
    {
        var cut = Render<ArcadiaAccordion>(p => p
            .AddChildContent<ArcadiaAccordionItem>(i => i.Add(x => x.Title, "FAQ Section").AddChildContent("Answer")));

        cut.Find(".arcadia-accordion-item__title").TextContent.Should().Be("FAQ Section");
    }

    [Fact]
    public void Panel_HasRegionRole()
    {
        var cut = Render<ArcadiaAccordion>(p => p
            .AddChildContent<ArcadiaAccordionItem>(i => i.Add(x => x.Title, "S1").AddChildContent("C1")));

        cut.Find("[role='region']").Should().NotBeNull();
    }
}
