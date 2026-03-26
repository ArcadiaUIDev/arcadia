using Bunit;
using FluentAssertions;
using Arcadia.FormBuilder.Components.Fields;
using Xunit;

namespace Arcadia.Tests.Unit.FormBuilder;

public class TimeFieldTests : Arcadia.Tests.Unit.ChartTestBase
{
    [Fact]
    public void Renders_WithLabel()
    {
        var cut = Render<TimeField>(p =>
            p.Add(c => c.Label, "Start Time")
             .Add(c => c.Value, (TimeSpan?)null));

        cut.Find("label").TextContent.Should().Contain("Start Time");
    }

    [Fact]
    public void Renders_WithValue()
    {
        var cut = Render<TimeField>(p =>
            p.Add(c => c.Label, "Start Time")
             .Add(c => c.Value, new TimeSpan(14, 30, 0)));

        cut.Find("input[type='time']").GetAttribute("value").Should().Be("14:30");
    }

    [Fact]
    public void Renders_Disabled()
    {
        var cut = Render<TimeField>(p =>
            p.Add(c => c.Label, "Start Time")
             .Add(c => c.Disabled, true)
             .Add(c => c.Value, (TimeSpan?)null));

        cut.Find("input[type='time']").HasAttribute("disabled").Should().BeTrue();
    }

    [Fact]
    public void ValueChanged_FiresOnChange()
    {
        TimeSpan? newValue = null;

        var cut = Render<TimeField>(p =>
            p.Add(c => c.Label, "Start Time")
             .Add(c => c.Value, (TimeSpan?)null)
             .Add(c => c.ValueChanged, (TimeSpan? v) => newValue = v));

        cut.Find("input[type='time']").Change("09:15");

        newValue.Should().NotBeNull();
        newValue!.Value.Hours.Should().Be(9);
        newValue!.Value.Minutes.Should().Be(15);
    }

    [Fact]
    public void Renders_NullValue_AsEmpty()
    {
        var cut = Render<TimeField>(p =>
            p.Add(c => c.Label, "Start Time")
             .Add(c => c.Value, (TimeSpan?)null));

        cut.Find("input[type='time']").GetAttribute("value").Should().BeNullOrEmpty();
    }

    [Fact]
    public void Label_LinkedToInput_ViaFor()
    {
        var cut = Render<TimeField>(p =>
            p.Add(c => c.Label, "Start Time")
             .Add(c => c.Value, (TimeSpan?)null));

        var inputId = cut.Find("input").GetAttribute("id");
        var labelFor = cut.Find("label").GetAttribute("for");
        labelFor.Should().Be(inputId);
    }

    [Fact]
    public void Renders_RequiredIndicator()
    {
        var cut = Render<TimeField>(p =>
            p.Add(c => c.Label, "Start Time")
             .Add(c => c.Required, true)
             .Add(c => c.Value, (TimeSpan?)null));

        cut.Find(".arcadia-field__required").TextContent.Should().Be("*");
        cut.Find("input").GetAttribute("aria-required").Should().Be("true");
    }
}
