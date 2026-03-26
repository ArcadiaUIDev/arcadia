using Bunit;
using FluentAssertions;
using Arcadia.FormBuilder.Components.Fields;
using Xunit;

namespace Arcadia.Tests.Unit.FormBuilder;

public class MaskedFieldTests : Arcadia.Tests.Unit.ChartTestBase
{
    [Fact]
    public void Renders_WithLabel()
    {
        var cut = Render<MaskedField>(p =>
            p.Add(c => c.Label, "Phone")
             .Add(c => c.Mask, "(###) ###-####")
             .Add(c => c.Value, ""));

        cut.Find("label").TextContent.Should().Contain("Phone");
    }

    [Fact]
    public void Renders_MaskedValue()
    {
        var cut = Render<MaskedField>(p =>
            p.Add(c => c.Label, "Phone")
             .Add(c => c.Mask, "(###) ###-####")
             .Add(c => c.Value, "5551234567"));

        cut.Find("input").GetAttribute("value").Should().Be("(555) 123-4567");
    }

    [Fact]
    public void Renders_Disabled()
    {
        var cut = Render<MaskedField>(p =>
            p.Add(c => c.Label, "Phone")
             .Add(c => c.Mask, "(###) ###-####")
             .Add(c => c.Disabled, true)
             .Add(c => c.Value, ""));

        cut.Find("input").HasAttribute("disabled").Should().BeTrue();
    }

    [Fact]
    public void Renders_MaskAsPlaceholder_WhenNoPlaceholder()
    {
        var cut = Render<MaskedField>(p =>
            p.Add(c => c.Label, "Phone")
             .Add(c => c.Mask, "(###) ###-####")
             .Add(c => c.Value, ""));

        cut.Find("input").GetAttribute("placeholder").Should().Be("(###) ###-####");
    }

    [Fact]
    public void Renders_CustomPlaceholder_OverridesMask()
    {
        var cut = Render<MaskedField>(p =>
            p.Add(c => c.Label, "Phone")
             .Add(c => c.Mask, "(###) ###-####")
             .Add(c => c.Placeholder, "Enter phone number")
             .Add(c => c.Value, ""));

        cut.Find("input").GetAttribute("placeholder").Should().Be("Enter phone number");
    }

    [Fact]
    public void Label_LinkedToInput_ViaFor()
    {
        var cut = Render<MaskedField>(p =>
            p.Add(c => c.Label, "Phone")
             .Add(c => c.Mask, "(###) ###-####")
             .Add(c => c.Value, ""));

        var inputId = cut.Find("input").GetAttribute("id");
        var labelFor = cut.Find("label").GetAttribute("for");
        labelFor.Should().Be(inputId);
    }

    [Fact]
    public void Renders_SSN_Mask()
    {
        var cut = Render<MaskedField>(p =>
            p.Add(c => c.Label, "SSN")
             .Add(c => c.Mask, "###-##-####")
             .Add(c => c.Value, "123456789"));

        cut.Find("input").GetAttribute("value").Should().Be("123-45-6789");
    }
}
