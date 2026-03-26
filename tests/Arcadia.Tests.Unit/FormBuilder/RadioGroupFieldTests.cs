using Bunit;
using FluentAssertions;
using Arcadia.FormBuilder.Components.Fields;
using Arcadia.FormBuilder.Schema;
using Xunit;

namespace Arcadia.Tests.Unit.FormBuilder;

public class RadioGroupFieldTests : Arcadia.Tests.Unit.ChartTestBase
{
    private static List<FieldOption> SampleOptions => new()
    {
        new() { Label = "Small", Value = "sm" },
        new() { Label = "Medium", Value = "md" },
        new() { Label = "Large", Value = "lg" }
    };

    [Fact]
    public void Renders_WithLegend()
    {
        var cut = Render<RadioGroupField>(p =>
            p.Add(c => c.Label, "Size")
             .Add(c => c.Value, (string?)null)
             .Add(c => c.Options, SampleOptions));

        cut.Find("legend").TextContent.Should().Contain("Size");
    }

    [Fact]
    public void Renders_AllOptions()
    {
        var cut = Render<RadioGroupField>(p =>
            p.Add(c => c.Label, "Size")
             .Add(c => c.Value, (string?)null)
             .Add(c => c.Options, SampleOptions));

        var radios = cut.FindAll("input[type='radio']");
        radios.Should().HaveCount(3);
    }

    [Fact]
    public void Renders_Disabled()
    {
        var cut = Render<RadioGroupField>(p =>
            p.Add(c => c.Label, "Size")
             .Add(c => c.Disabled, true)
             .Add(c => c.Value, (string?)null)
             .Add(c => c.Options, SampleOptions));

        var radios = cut.FindAll("input[type='radio']");
        radios.Should().AllSatisfy(r => r.HasAttribute("disabled").Should().BeTrue());
    }

    [Fact]
    public void ValueChanged_FiresOnSelection()
    {
        string? newValue = null;

        var cut = Render<RadioGroupField>(p =>
            p.Add(c => c.Label, "Size")
             .Add(c => c.Value, (string?)null)
             .Add(c => c.Options, SampleOptions)
             .Add(c => c.ValueChanged, (string? v) => newValue = v));

        cut.FindAll("input[type='radio']")[1].Change("md");

        newValue.Should().Be("md");
    }

    [Fact]
    public void SelectedOption_HasChecked()
    {
        var cut = Render<RadioGroupField>(p =>
            p.Add(c => c.Label, "Size")
             .Add(c => c.Value, "md")
             .Add(c => c.Options, SampleOptions));

        var radios = cut.FindAll("input[type='radio']");
        radios[1].HasAttribute("checked").Should().BeTrue();
    }

    [Fact]
    public void HasRole_Radiogroup()
    {
        var cut = Render<RadioGroupField>(p =>
            p.Add(c => c.Label, "Size")
             .Add(c => c.Value, (string?)null)
             .Add(c => c.Options, SampleOptions));

        cut.Find("[role='radiogroup']").Should().NotBeNull();
    }

    [Fact]
    public void Renders_RequiredIndicator()
    {
        var cut = Render<RadioGroupField>(p =>
            p.Add(c => c.Label, "Size")
             .Add(c => c.Required, true)
             .Add(c => c.Value, (string?)null)
             .Add(c => c.Options, SampleOptions));

        cut.Find(".arcadia-field__required").TextContent.Should().Be("*");
        cut.Find("[role='radiogroup']").GetAttribute("aria-required").Should().Be("true");
    }
}
