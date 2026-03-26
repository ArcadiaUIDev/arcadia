using Bunit;
using FluentAssertions;
using Arcadia.FormBuilder.Components.Fields;
using Xunit;

namespace Arcadia.Tests.Unit.FormBuilder;

public class NumberFieldTests : Arcadia.Tests.Unit.ChartTestBase
{
    [Fact]
    public void Renders_WithLabel()
    {
        var cut = Render<NumberField>(p =>
            p.Add(c => c.Label, "Quantity")
             .Add(c => c.Value, (double?)null));

        cut.Find("label").TextContent.Should().Contain("Quantity");
    }

    [Fact]
    public void Renders_WithValue()
    {
        var cut = Render<NumberField>(p =>
            p.Add(c => c.Label, "Quantity")
             .Add(c => c.Value, 42.0));

        cut.Find("input").GetAttribute("value").Should().Be("42");
    }

    [Fact]
    public void Renders_Disabled()
    {
        var cut = Render<NumberField>(p =>
            p.Add(c => c.Label, "Quantity")
             .Add(c => c.Disabled, true)
             .Add(c => c.Value, (double?)null));

        cut.Find("input").HasAttribute("disabled").Should().BeTrue();
    }

    [Fact]
    public void Renders_MinMaxStep()
    {
        var cut = Render<NumberField>(p =>
            p.Add(c => c.Label, "Age")
             .Add(c => c.Value, 25.0)
             .Add(c => c.Min, 0.0)
             .Add(c => c.Max, 120.0)
             .Add(c => c.Step, "1"));

        var input = cut.Find("input");
        input.GetAttribute("min").Should().Be("0");
        input.GetAttribute("max").Should().Be("120");
        input.GetAttribute("step").Should().Be("1");
    }

    [Fact]
    public void ValueChanged_FiresOnInput()
    {
        double? newValue = null;

        var cut = Render<NumberField>(p =>
            p.Add(c => c.Label, "Quantity")
             .Add(c => c.Value, (double?)null)
             .Add(c => c.ValueChanged, (double? v) => newValue = v));

        cut.Find("input").Input("7");

        newValue.Should().Be(7);
    }

    [Fact]
    public void Renders_RequiredIndicator()
    {
        var cut = Render<NumberField>(p =>
            p.Add(c => c.Label, "Quantity")
             .Add(c => c.Required, true)
             .Add(c => c.Value, (double?)null));

        cut.Find(".arcadia-field__required").TextContent.Should().Be("*");
        cut.Find("input").GetAttribute("aria-required").Should().Be("true");
    }

    [Fact]
    public void Label_LinkedToInput_ViaFor()
    {
        var cut = Render<NumberField>(p =>
            p.Add(c => c.Label, "Quantity")
             .Add(c => c.Value, (double?)null));

        var inputId = cut.Find("input").GetAttribute("id");
        var labelFor = cut.Find("label").GetAttribute("for");
        labelFor.Should().Be(inputId);
    }
}
