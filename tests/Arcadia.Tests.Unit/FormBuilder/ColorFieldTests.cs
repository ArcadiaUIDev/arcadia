using Bunit;
using FluentAssertions;
using Arcadia.FormBuilder.Components.Fields;
using Xunit;

namespace Arcadia.Tests.Unit.FormBuilder;

public class ColorFieldTests : Arcadia.Tests.Unit.ChartTestBase
{
    [Fact]
    public void Renders_WithLabel()
    {
        var cut = Render<ColorField>(p =>
            p.Add(c => c.Label, "Theme Color")
             .Add(c => c.Value, "#ff0000"));

        cut.Find("label").TextContent.Should().Contain("Theme Color");
    }

    [Fact]
    public void Renders_ColorAndTextInputs()
    {
        var cut = Render<ColorField>(p =>
            p.Add(c => c.Label, "Color")
             .Add(c => c.Value, "#ff0000"));

        cut.FindAll("input").Should().HaveCount(2);
        cut.Find("input[type='color']").Should().NotBeNull();
    }

    [Fact]
    public void Renders_Disabled()
    {
        var cut = Render<ColorField>(p =>
            p.Add(c => c.Label, "Color")
             .Add(c => c.Disabled, true)
             .Add(c => c.Value, "#000000"));

        var inputs = cut.FindAll("input");
        inputs.Should().AllSatisfy(i => i.HasAttribute("disabled").Should().BeTrue());
    }

    [Fact]
    public void Renders_WithValue()
    {
        var cut = Render<ColorField>(p =>
            p.Add(c => c.Label, "Color")
             .Add(c => c.Value, "#ff5733"));

        cut.Find("input[type='color']").GetAttribute("value").Should().Be("#ff5733");
    }

    [Fact]
    public void ValueChanged_FiresOnColorInput()
    {
        string? newValue = null;

        var cut = Render<ColorField>(p =>
            p.Add(c => c.Label, "Color")
             .Add(c => c.Value, "#000000")
             .Add(c => c.ValueChanged, (string? v) => newValue = v));

        cut.Find("input[type='color']").Input("#00ff00");

        newValue.Should().Be("#00ff00");
    }

    [Fact]
    public void Renders_DefaultColor_WhenNull()
    {
        var cut = Render<ColorField>(p =>
            p.Add(c => c.Label, "Color")
             .Add(c => c.Value, (string?)null));

        cut.Find("input[type='color']").GetAttribute("value").Should().Be("#000000");
    }

    [Fact]
    public void TextInput_HasAriaLabel()
    {
        var cut = Render<ColorField>(p =>
            p.Add(c => c.Label, "Theme Color")
             .Add(c => c.Value, "#ff0000"));

        cut.Find(".arcadia-field__color-text").GetAttribute("aria-label")
           .Should().Be("Theme Color hex value");
    }
}
