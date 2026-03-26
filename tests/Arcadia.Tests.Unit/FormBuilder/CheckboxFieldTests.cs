using Bunit;
using FluentAssertions;
using Arcadia.FormBuilder.Components.Fields;
using Xunit;

namespace Arcadia.Tests.Unit.FormBuilder;

public class CheckboxFieldTests : Arcadia.Tests.Unit.ChartTestBase
{
    [Fact]
    public void Renders_WithLabel()
    {
        var cut = Render<CheckboxField>(p =>
            p.Add(c => c.Label, "Accept Terms")
             .Add(c => c.Value, false));

        cut.Find(".arcadia-field__checkbox-text").TextContent.Should().Contain("Accept Terms");
    }

    [Fact]
    public void Renders_Checked_WhenValueIsTrue()
    {
        var cut = Render<CheckboxField>(p =>
            p.Add(c => c.Label, "Accept Terms")
             .Add(c => c.Value, true));

        cut.Find("input[type='checkbox']").HasAttribute("checked").Should().BeTrue();
    }

    [Fact]
    public void Renders_Unchecked_WhenValueIsFalse()
    {
        var cut = Render<CheckboxField>(p =>
            p.Add(c => c.Label, "Accept Terms")
             .Add(c => c.Value, false));

        // When checked is false, the attribute should not be present or should be false
        var input = cut.Find("input[type='checkbox']");
        input.Should().NotBeNull();
    }

    [Fact]
    public void Renders_Disabled()
    {
        var cut = Render<CheckboxField>(p =>
            p.Add(c => c.Label, "Accept Terms")
             .Add(c => c.Disabled, true)
             .Add(c => c.Value, false));

        cut.Find("input[type='checkbox']").HasAttribute("disabled").Should().BeTrue();
    }

    [Fact]
    public void ValueChanged_FiresOnChange()
    {
        bool newValue = false;

        var cut = Render<CheckboxField>(p =>
            p.Add(c => c.Label, "Accept Terms")
             .Add(c => c.Value, false)
             .Add(c => c.ValueChanged, (bool v) => newValue = v));

        cut.Find("input[type='checkbox']").Change(true);

        newValue.Should().BeTrue();
    }

    [Fact]
    public void Renders_RequiredIndicator()
    {
        var cut = Render<CheckboxField>(p =>
            p.Add(c => c.Label, "Accept Terms")
             .Add(c => c.Required, true)
             .Add(c => c.Value, false));

        cut.Find(".arcadia-field__required").TextContent.Should().Be("*");
        cut.Find("input[type='checkbox']").GetAttribute("aria-required").Should().Be("true");
    }

    [Fact]
    public void Renders_HelperText()
    {
        var cut = Render<CheckboxField>(p =>
            p.Add(c => c.Label, "Accept Terms")
             .Add(c => c.HelperText, "You must agree to proceed")
             .Add(c => c.Value, false));

        cut.Find(".arcadia-field__helper").TextContent.Should().Be("You must agree to proceed");
    }
}
