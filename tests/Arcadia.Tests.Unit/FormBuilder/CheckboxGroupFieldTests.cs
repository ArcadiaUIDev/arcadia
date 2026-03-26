using Bunit;
using FluentAssertions;
using Arcadia.FormBuilder.Components.Fields;
using Arcadia.FormBuilder.Schema;
using Xunit;

namespace Arcadia.Tests.Unit.FormBuilder;

public class CheckboxGroupFieldTests : Arcadia.Tests.Unit.ChartTestBase
{
    private static List<FieldOption> SampleOptions => new()
    {
        new() { Label = "Red", Value = "red" },
        new() { Label = "Blue", Value = "blue" },
        new() { Label = "Green", Value = "green" }
    };

    [Fact]
    public void Renders_WithLegend()
    {
        var cut = Render<CheckboxGroupField>(p =>
            p.Add(c => c.Label, "Favorite Colors")
             .Add(c => c.Values, new List<string>())
             .Add(c => c.Options, SampleOptions));

        cut.Find("legend").TextContent.Should().Contain("Favorite Colors");
    }

    [Fact]
    public void Renders_AllOptions()
    {
        var cut = Render<CheckboxGroupField>(p =>
            p.Add(c => c.Label, "Colors")
             .Add(c => c.Values, new List<string>())
             .Add(c => c.Options, SampleOptions));

        cut.FindAll("input[type='checkbox']").Should().HaveCount(3);
    }

    [Fact]
    public void Renders_Disabled()
    {
        var cut = Render<CheckboxGroupField>(p =>
            p.Add(c => c.Label, "Colors")
             .Add(c => c.Disabled, true)
             .Add(c => c.Values, new List<string>())
             .Add(c => c.Options, SampleOptions));

        var checkboxes = cut.FindAll("input[type='checkbox']");
        checkboxes.Should().AllSatisfy(cb => cb.HasAttribute("disabled").Should().BeTrue());
    }

    [Fact]
    public void SelectedValues_AreChecked()
    {
        var cut = Render<CheckboxGroupField>(p =>
            p.Add(c => c.Label, "Colors")
             .Add(c => c.Values, new List<string> { "red", "green" })
             .Add(c => c.Options, SampleOptions));

        var checkboxes = cut.FindAll("input[type='checkbox']");
        checkboxes[0].HasAttribute("checked").Should().BeTrue();  // red
        checkboxes[1].HasAttribute("checked").Should().BeFalse(); // blue
        checkboxes[2].HasAttribute("checked").Should().BeTrue();  // green
    }

    [Fact]
    public void ValuesChanged_FiresOnToggle()
    {
        List<string>? newValues = null;

        var cut = Render<CheckboxGroupField>(p =>
            p.Add(c => c.Label, "Colors")
             .Add(c => c.Values, new List<string> { "red" })
             .Add(c => c.Options, SampleOptions)
             .Add(c => c.ValuesChanged, (List<string> v) => newValues = v));

        cut.FindAll("input[type='checkbox']")[1].Change(true); // check blue

        newValues.Should().NotBeNull();
        newValues.Should().Contain("blue");
    }

    [Fact]
    public void HasRole_Group()
    {
        var cut = Render<CheckboxGroupField>(p =>
            p.Add(c => c.Label, "Colors")
             .Add(c => c.Values, new List<string>())
             .Add(c => c.Options, SampleOptions));

        cut.Find("[role='group']").Should().NotBeNull();
    }

    [Fact]
    public void Renders_RequiredIndicator()
    {
        var cut = Render<CheckboxGroupField>(p =>
            p.Add(c => c.Label, "Colors")
             .Add(c => c.Required, true)
             .Add(c => c.Values, new List<string>())
             .Add(c => c.Options, SampleOptions));

        cut.Find(".arcadia-field__required").TextContent.Should().Be("*");
        cut.Find("[role='group']").GetAttribute("aria-required").Should().Be("true");
    }
}
