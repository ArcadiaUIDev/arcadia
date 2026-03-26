using Bunit;
using FluentAssertions;
using Arcadia.FormBuilder.Components.Fields;
using Xunit;

namespace Arcadia.Tests.Unit.FormBuilder;

public class PasswordFieldTests : Arcadia.Tests.Unit.ChartTestBase
{
    [Fact]
    public void Renders_WithLabel()
    {
        var cut = Render<PasswordField>(p =>
            p.Add(c => c.Label, "Password")
             .Add(c => c.Value, ""));

        cut.Find("label").TextContent.Should().Contain("Password");
    }

    [Fact]
    public void Renders_HiddenByDefault()
    {
        var cut = Render<PasswordField>(p =>
            p.Add(c => c.Label, "Password")
             .Add(c => c.Value, "secret"));

        cut.Find("input").GetAttribute("type").Should().Be("password");
    }

    [Fact]
    public void Renders_Disabled()
    {
        var cut = Render<PasswordField>(p =>
            p.Add(c => c.Label, "Password")
             .Add(c => c.Disabled, true)
             .Add(c => c.Value, ""));

        cut.Find("input").HasAttribute("disabled").Should().BeTrue();
    }

    [Fact]
    public void ValueChanged_FiresOnInput()
    {
        string? newValue = null;

        var cut = Render<PasswordField>(p =>
            p.Add(c => c.Label, "Password")
             .Add(c => c.Value, "")
             .Add(c => c.ValueChanged, (string? v) => newValue = v));

        cut.Find("input").Input("newpass");

        newValue.Should().Be("newpass");
    }

    [Fact]
    public void Toggle_ShowsPassword()
    {
        var cut = Render<PasswordField>(p =>
            p.Add(c => c.Label, "Password")
             .Add(c => c.Value, "secret"));

        cut.Find(".arcadia-field__password-toggle").Click();

        cut.Find("input").GetAttribute("type").Should().Be("text");
    }

    [Fact]
    public void Toggle_HidesPassword_AfterSecondClick()
    {
        var cut = Render<PasswordField>(p =>
            p.Add(c => c.Label, "Password")
             .Add(c => c.Value, "secret"));

        var toggle = cut.Find(".arcadia-field__password-toggle");
        toggle.Click(); // show
        toggle.Click(); // hide

        cut.Find("input").GetAttribute("type").Should().Be("password");
    }

    [Fact]
    public void StrengthMeter_Visible_WhenEnabled()
    {
        var cut = Render<PasswordField>(p =>
            p.Add(c => c.Label, "Password")
             .Add(c => c.Value, "Str0ng!Pass")
             .Add(c => c.ShowStrength, true));

        cut.FindAll(".arcadia-field__password-strength").Should().HaveCount(1);
    }

    [Fact]
    public void StrengthMeter_Hidden_WhenDisabledParam()
    {
        var cut = Render<PasswordField>(p =>
            p.Add(c => c.Label, "Password")
             .Add(c => c.Value, "Str0ng!Pass")
             .Add(c => c.ShowStrength, false));

        cut.FindAll(".arcadia-field__password-strength").Should().HaveCount(0);
    }

    [Fact]
    public void Autocomplete_NewPassword_ByDefault()
    {
        var cut = Render<PasswordField>(p =>
            p.Add(c => c.Label, "Password")
             .Add(c => c.Value, ""));

        cut.Find("input").GetAttribute("autocomplete").Should().Be("new-password");
    }

    [Fact]
    public void Autocomplete_CurrentPassword_WhenIsNewFalse()
    {
        var cut = Render<PasswordField>(p =>
            p.Add(c => c.Label, "Password")
             .Add(c => c.Value, "")
             .Add(c => c.IsNew, false));

        cut.Find("input").GetAttribute("autocomplete").Should().Be("current-password");
    }
}
