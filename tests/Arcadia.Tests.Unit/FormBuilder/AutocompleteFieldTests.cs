using Bunit;
using FluentAssertions;
using Arcadia.FormBuilder.Components.Fields;
using Arcadia.FormBuilder.Schema;
using Xunit;

namespace Arcadia.Tests.Unit.FormBuilder;

public class AutocompleteFieldTests : Arcadia.Tests.Unit.ChartTestBase
{
    private static List<FieldOption> SampleOptions => new()
    {
        new() { Label = "Apple", Value = "apple" },
        new() { Label = "Banana", Value = "banana" },
        new() { Label = "Cherry", Value = "cherry" }
    };

    [Fact]
    public void Renders_WithLabel()
    {
        var cut = Render<AutocompleteField>(p =>
            p.Add(c => c.Label, "Fruit")
             .Add(c => c.Value, "")
             .Add(c => c.Options, SampleOptions));

        cut.Find("label").TextContent.Should().Contain("Fruit");
    }

    [Fact]
    public void Renders_WithValue()
    {
        var cut = Render<AutocompleteField>(p =>
            p.Add(c => c.Label, "Fruit")
             .Add(c => c.Value, "apple")
             .Add(c => c.Options, SampleOptions));

        cut.Find("input").GetAttribute("value").Should().Be("apple");
    }

    [Fact]
    public void Renders_Disabled()
    {
        var cut = Render<AutocompleteField>(p =>
            p.Add(c => c.Label, "Fruit")
             .Add(c => c.Disabled, true)
             .Add(c => c.Value, "")
             .Add(c => c.Options, SampleOptions));

        cut.Find("input").HasAttribute("disabled").Should().BeTrue();
    }

    [Fact]
    public void HasRole_Combobox()
    {
        var cut = Render<AutocompleteField>(p =>
            p.Add(c => c.Label, "Fruit")
             .Add(c => c.Value, "")
             .Add(c => c.Options, SampleOptions));

        cut.Find("input").GetAttribute("role").Should().Be("combobox");
    }

    [Fact]
    public void ValueChanged_FiresOnInput()
    {
        string? newValue = null;

        var cut = Render<AutocompleteField>(p =>
            p.Add(c => c.Label, "Fruit")
             .Add(c => c.Value, "")
             .Add(c => c.Options, SampleOptions)
             .Add(c => c.ValueChanged, (string? v) => newValue = v));

        cut.Find("input").Input("ban");

        newValue.Should().Be("ban");
    }

    [Fact]
    public void Label_LinkedToInput_ViaFor()
    {
        var cut = Render<AutocompleteField>(p =>
            p.Add(c => c.Label, "Fruit")
             .Add(c => c.Value, "")
             .Add(c => c.Options, SampleOptions));

        var inputId = cut.Find("input").GetAttribute("id");
        var labelFor = cut.Find("label").GetAttribute("for");
        labelFor.Should().Be(inputId);
    }

    [Fact]
    public void Renders_Placeholder()
    {
        var cut = Render<AutocompleteField>(p =>
            p.Add(c => c.Label, "Fruit")
             .Add(c => c.Placeholder, "Search fruits...")
             .Add(c => c.Value, "")
             .Add(c => c.Options, SampleOptions));

        cut.Find("input").GetAttribute("placeholder").Should().Be("Search fruits...");
    }
}
