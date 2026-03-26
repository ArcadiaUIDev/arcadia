using Bunit;
using FluentAssertions;
using Arcadia.FormBuilder.Components.Fields;
using Arcadia.FormBuilder.Schema;
using Xunit;

namespace Arcadia.Tests.Unit.FormBuilder;

public class SelectFieldTests : Arcadia.Tests.Unit.ChartTestBase
{
    private static List<FieldOption> SampleOptions => new()
    {
        new() { Label = "Red", Value = "red" },
        new() { Label = "Green", Value = "green" },
        new() { Label = "Blue", Value = "blue" }
    };

    [Fact]
    public void Renders_WithLabel()
    {
        var cut = Render<SelectField>(p =>
            p.Add(c => c.Label, "Color")
             .Add(c => c.Value, (string?)null)
             .Add(c => c.Options, SampleOptions));

        cut.Find("label").TextContent.Should().Contain("Color");
    }

    [Fact]
    public void Renders_Options()
    {
        var cut = Render<SelectField>(p =>
            p.Add(c => c.Label, "Color")
             .Add(c => c.Value, (string?)null)
             .Add(c => c.Options, SampleOptions));

        var options = cut.FindAll("option");
        options.Should().Contain(o => o.TextContent.Trim() == "Red");
        options.Should().Contain(o => o.TextContent.Trim() == "Green");
        options.Should().Contain(o => o.TextContent.Trim() == "Blue");
    }

    [Fact]
    public void Renders_Disabled()
    {
        var cut = Render<SelectField>(p =>
            p.Add(c => c.Label, "Color")
             .Add(c => c.Disabled, true)
             .Add(c => c.Value, (string?)null)
             .Add(c => c.Options, SampleOptions));

        cut.Find("select").HasAttribute("disabled").Should().BeTrue();
    }

    [Fact]
    public void ValueChanged_FiresOnChange()
    {
        string? newValue = null;

        var cut = Render<SelectField>(p =>
            p.Add(c => c.Label, "Color")
             .Add(c => c.Value, (string?)null)
             .Add(c => c.Options, SampleOptions)
             .Add(c => c.ValueChanged, (string? v) => newValue = v));

        cut.Find("select").Change("green");

        newValue.Should().Be("green");
    }

    [Fact]
    public void Renders_Placeholder()
    {
        var cut = Render<SelectField>(p =>
            p.Add(c => c.Label, "Color")
             .Add(c => c.Value, (string?)null)
             .Add(c => c.Placeholder, "Choose a color...")
             .Add(c => c.Options, SampleOptions));

        var placeholderOption = cut.FindAll("option").First();
        placeholderOption.TextContent.Trim().Should().Be("Choose a color...");
    }

    [Fact]
    public void Renders_RequiredIndicator()
    {
        var cut = Render<SelectField>(p =>
            p.Add(c => c.Label, "Color")
             .Add(c => c.Required, true)
             .Add(c => c.Value, (string?)null)
             .Add(c => c.Options, SampleOptions));

        cut.Find(".arcadia-field__required").TextContent.Should().Be("*");
        cut.Find("select").GetAttribute("aria-required").Should().Be("true");
    }

    [Fact]
    public void Label_LinkedToSelect_ViaFor()
    {
        var cut = Render<SelectField>(p =>
            p.Add(c => c.Label, "Color")
             .Add(c => c.Value, (string?)null)
             .Add(c => c.Options, SampleOptions));

        var selectId = cut.Find("select").GetAttribute("id");
        var labelFor = cut.Find("label").GetAttribute("for");
        labelFor.Should().Be(selectId);
    }
}
