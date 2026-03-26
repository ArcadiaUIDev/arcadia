using Bunit;
using FluentAssertions;
using Arcadia.FormBuilder.Components.Fields;
using Xunit;

namespace Arcadia.Tests.Unit.FormBuilder;

public class SwitchFieldTests : Arcadia.Tests.Unit.ChartTestBase
{
    [Fact]
    public void Renders_WithLabel()
    {
        var cut = Render<SwitchField>(p =>
            p.Add(c => c.Label, "Notifications")
             .Add(c => c.Value, false));

        cut.Find(".arcadia-field__switch-text").TextContent.Should().Contain("Notifications");
    }

    [Fact]
    public void Renders_Checked_WhenValueIsTrue()
    {
        var cut = Render<SwitchField>(p =>
            p.Add(c => c.Label, "Notifications")
             .Add(c => c.Value, true));

        var input = cut.Find("input[role='switch']");
        input.GetAttribute("aria-checked").Should().Be("true");
    }

    [Fact]
    public void Renders_Unchecked_WhenValueIsFalse()
    {
        var cut = Render<SwitchField>(p =>
            p.Add(c => c.Label, "Notifications")
             .Add(c => c.Value, false));

        var input = cut.Find("input[role='switch']");
        input.GetAttribute("aria-checked").Should().Be("false");
    }

    [Fact]
    public void Renders_Disabled()
    {
        var cut = Render<SwitchField>(p =>
            p.Add(c => c.Label, "Notifications")
             .Add(c => c.Disabled, true)
             .Add(c => c.Value, false));

        cut.Find("input[role='switch']").HasAttribute("disabled").Should().BeTrue();
    }

    [Fact]
    public void ValueChanged_FiresOnChange()
    {
        bool newValue = false;

        var cut = Render<SwitchField>(p =>
            p.Add(c => c.Label, "Notifications")
             .Add(c => c.Value, false)
             .Add(c => c.ValueChanged, (bool v) => newValue = v));

        cut.Find("input[role='switch']").Change(true);

        newValue.Should().BeTrue();
    }

    [Fact]
    public void HasRole_Switch()
    {
        var cut = Render<SwitchField>(p =>
            p.Add(c => c.Label, "Notifications")
             .Add(c => c.Value, false));

        cut.Find("input").GetAttribute("role").Should().Be("switch");
    }

    [Fact]
    public void Renders_HelperText()
    {
        var cut = Render<SwitchField>(p =>
            p.Add(c => c.Label, "Notifications")
             .Add(c => c.HelperText, "Enable push notifications")
             .Add(c => c.Value, false));

        cut.Find(".arcadia-field__helper").TextContent.Should().Be("Enable push notifications");
    }
}
