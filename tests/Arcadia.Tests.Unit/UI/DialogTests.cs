using Bunit;
using FluentAssertions;
using Xunit;
using Arcadia.UI.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Arcadia.Tests.Unit.UI;

public class DialogTests : ChartTestBase
{
    [Fact]
    public void Visible_False_RendersNothing()
    {
        var cut = Render<ArcadiaDialog>(p => p
            .Add(c => c.Visible, false)
            .Add(c => c.Title, "Test"));

        cut.Markup.Trim().Should().BeEmpty();
    }

    [Fact]
    public void Visible_True_ShowsOverlayAndDialog()
    {
        var cut = Render<ArcadiaDialog>(p => p
            .Add(c => c.Visible, true)
            .Add(c => c.Title, "Test"));

        cut.Find(".arcadia-dialog-overlay").Should().NotBeNull();
        cut.Find(".arcadia-dialog").Should().NotBeNull();
    }

    [Fact]
    public void Title_RendersInHeader()
    {
        var cut = Render<ArcadiaDialog>(p => p
            .Add(c => c.Visible, true)
            .Add(c => c.Title, "My Dialog"));

        cut.Find(".arcadia-dialog__title").TextContent.Should().Be("My Dialog");
    }

    [Fact]
    public void CloseButton_FiresVisibleChanged()
    {
        var visibleChanged = false;
        var cut = Render<ArcadiaDialog>(p => p
            .Add(c => c.Visible, true)
            .Add(c => c.Title, "Test")
            .Add(c => c.ShowCloseButton, true)
            .Add(c => c.VisibleChanged, v => visibleChanged = true));

        cut.Find(".arcadia-dialog__close").Click();

        visibleChanged.Should().BeTrue();
    }

    [Fact]
    public void CloseOnEscape_ClosesDialog()
    {
        var closed = false;
        var cut = Render<ArcadiaDialog>(p => p
            .Add(c => c.Visible, true)
            .Add(c => c.CloseOnEscape, true)
            .Add(c => c.VisibleChanged, v => closed = true));

        cut.Find(".arcadia-dialog-overlay").KeyDown(new KeyboardEventArgs { Key = "Escape" });

        closed.Should().BeTrue();
    }

    [Fact]
    public void CloseOnOverlayClick_ClosesDialog()
    {
        var closed = false;
        var cut = Render<ArcadiaDialog>(p => p
            .Add(c => c.Visible, true)
            .Add(c => c.CloseOnOverlayClick, true)
            .Add(c => c.VisibleChanged, v => closed = true));

        cut.Find(".arcadia-dialog-overlay").Click();

        closed.Should().BeTrue();
    }

    [Fact]
    public void FooterTemplate_RendersInFooter()
    {
        var cut = Render<ArcadiaDialog>(p => p
            .Add(c => c.Visible, true)
            .Add(c => c.FooterTemplate, "<button>OK</button>"));

        cut.Find(".arcadia-dialog__footer").InnerHtml.Should().Contain("OK");
    }

    [Fact]
    public void Aria_RoleDialog_AndAriaModal()
    {
        var cut = Render<ArcadiaDialog>(p => p
            .Add(c => c.Visible, true)
            .Add(c => c.Title, "Accessible"));

        var dialog = cut.Find("[role='dialog']");
        dialog.Should().NotBeNull();
        dialog.GetAttribute("aria-modal").Should().Be("true");
    }
}
