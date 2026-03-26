using Bunit;
using FluentAssertions;
using Arcadia.FormBuilder.Components.Fields;
using Xunit;

namespace Arcadia.Tests.Unit.FormBuilder;

public class DateFieldTests : Arcadia.Tests.Unit.ChartTestBase
{
    [Fact]
    public void Renders_WithLabel()
    {
        var cut = Render<DateField>(p =>
            p.Add(c => c.Label, "Birth Date")
             .Add(c => c.Value, (DateTime?)null));

        cut.Find("label").TextContent.Should().Contain("Birth Date");
    }

    [Fact]
    public void Renders_WithValue()
    {
        var cut = Render<DateField>(p =>
            p.Add(c => c.Label, "Birth Date")
             .Add(c => c.Value, new DateTime(2026, 3, 15)));

        cut.Find("input").GetAttribute("value").Should().Be("2026-03-15");
    }

    [Fact]
    public void Renders_Disabled()
    {
        var cut = Render<DateField>(p =>
            p.Add(c => c.Label, "Birth Date")
             .Add(c => c.Disabled, true)
             .Add(c => c.Value, (DateTime?)null));

        cut.Find("input").HasAttribute("disabled").Should().BeTrue();
    }

    [Fact]
    public void ValueChanged_FiresOnChange()
    {
        DateTime? newValue = null;

        var cut = Render<DateField>(p =>
            p.Add(c => c.Label, "Birth Date")
             .Add(c => c.Value, (DateTime?)null)
             .Add(c => c.ValueChanged, (DateTime? v) => newValue = v));

        cut.Find("input").Change("2026-06-01");

        newValue.Should().NotBeNull();
        newValue!.Value.Year.Should().Be(2026);
        newValue!.Value.Month.Should().Be(6);
        newValue!.Value.Day.Should().Be(1);
    }

    [Fact]
    public void Renders_RequiredIndicator()
    {
        var cut = Render<DateField>(p =>
            p.Add(c => c.Label, "Birth Date")
             .Add(c => c.Required, true)
             .Add(c => c.Value, (DateTime?)null));

        cut.Find(".arcadia-field__required").TextContent.Should().Be("*");
        cut.Find("input").GetAttribute("aria-required").Should().Be("true");
    }

    [Fact]
    public void Label_LinkedToInput_ViaFor()
    {
        var cut = Render<DateField>(p =>
            p.Add(c => c.Label, "Birth Date")
             .Add(c => c.Value, (DateTime?)null));

        var inputId = cut.Find("input").GetAttribute("id");
        var labelFor = cut.Find("label").GetAttribute("for");
        labelFor.Should().Be(inputId);
    }

    [Fact]
    public void Renders_NullValue_AsEmpty()
    {
        var cut = Render<DateField>(p =>
            p.Add(c => c.Label, "Birth Date")
             .Add(c => c.Value, (DateTime?)null));

        cut.Find("input").GetAttribute("value").Should().BeNullOrEmpty();
    }
}
