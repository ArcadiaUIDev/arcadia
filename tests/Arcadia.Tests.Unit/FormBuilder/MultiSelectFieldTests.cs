using Bunit;
using FluentAssertions;
using Arcadia.FormBuilder.Components.Fields;
using Arcadia.FormBuilder.Schema;
using Xunit;

namespace Arcadia.Tests.Unit.FormBuilder;

public class MultiSelectFieldTests : Arcadia.Tests.Unit.ChartTestBase
{
    private static List<FieldOption> SampleOptions => new()
    {
        new() { Label = "Alpha", Value = "a" },
        new() { Label = "Beta", Value = "b" },
        new() { Label = "Gamma", Value = "c" }
    };

    [Fact]
    public void Renders_WithLabel()
    {
        var cut = Render<MultiSelectField>(p =>
            p.Add(c => c.Label, "Tags")
             .Add(c => c.Values, new List<string>())
             .Add(c => c.Options, SampleOptions));

        cut.Find("label").TextContent.Should().Contain("Tags");
    }

    [Fact]
    public void Renders_Tags_ForSelectedValues()
    {
        var cut = Render<MultiSelectField>(p =>
            p.Add(c => c.Label, "Tags")
             .Add(c => c.Values, new List<string> { "a", "b" })
             .Add(c => c.Options, SampleOptions));

        cut.FindAll(".arcadia-field__multiselect-tag").Should().HaveCount(2);
    }

    [Fact]
    public void Renders_Disabled()
    {
        var cut = Render<MultiSelectField>(p =>
            p.Add(c => c.Label, "Tags")
             .Add(c => c.Disabled, true)
             .Add(c => c.Values, new List<string>())
             .Add(c => c.Options, SampleOptions));

        cut.Find("select").HasAttribute("disabled").Should().BeTrue();
    }

    [Fact]
    public void ValuesChanged_FiresOnAdd()
    {
        List<string>? newValues = null;

        var cut = Render<MultiSelectField>(p =>
            p.Add(c => c.Label, "Tags")
             .Add(c => c.Values, new List<string>())
             .Add(c => c.Options, SampleOptions)
             .Add(c => c.ValuesChanged, (List<string> v) => newValues = v));

        cut.Find("select").Change("a");

        newValues.Should().NotBeNull();
        newValues.Should().Contain("a");
    }

    [Fact]
    public void SelectedOptions_NotInDropdown()
    {
        var cut = Render<MultiSelectField>(p =>
            p.Add(c => c.Label, "Tags")
             .Add(c => c.Values, new List<string> { "a" })
             .Add(c => c.Options, SampleOptions));

        // Alpha (value "a") is selected, so only Beta and Gamma should be in dropdown
        var dropdownOptions = cut.FindAll("select option");
        dropdownOptions.Should().NotContain(o => o.GetAttribute("value") == "a");
    }

    [Fact]
    public void RemoveButton_NotShown_WhenDisabled()
    {
        var cut = Render<MultiSelectField>(p =>
            p.Add(c => c.Label, "Tags")
             .Add(c => c.Disabled, true)
             .Add(c => c.Values, new List<string> { "a" })
             .Add(c => c.Options, SampleOptions));

        cut.FindAll(".arcadia-field__multiselect-tag-remove").Should().HaveCount(0);
    }

    [Fact]
    public void HasRole_Listbox()
    {
        var cut = Render<MultiSelectField>(p =>
            p.Add(c => c.Label, "Tags")
             .Add(c => c.Values, new List<string>())
             .Add(c => c.Options, SampleOptions));

        cut.Find("[role='listbox']").Should().NotBeNull();
    }

    [Fact]
    public void Renders_Placeholder_InDropdown()
    {
        var cut = Render<MultiSelectField>(p =>
            p.Add(c => c.Label, "Tags")
             .Add(c => c.Placeholder, "Pick items...")
             .Add(c => c.Values, new List<string>())
             .Add(c => c.Options, SampleOptions));

        var firstOption = cut.FindAll("select option").First();
        firstOption.TextContent.Trim().Should().Be("Pick items...");
    }
}
