using Bunit;
using FluentAssertions;
using Arcadia.FormBuilder.Components.Fields;
using Xunit;

namespace Arcadia.Tests.Unit.FormBuilder;

public class TextAreaFieldTests : Arcadia.Tests.Unit.ChartTestBase
{
    [Fact]
    public void Renders_WithLabel()
    {
        var cut = Render<TextAreaField>(p =>
            p.Add(c => c.Label, "Comments")
             .Add(c => c.Value, ""));

        cut.Find("label").TextContent.Should().Contain("Comments");
    }

    [Fact]
    public void Renders_WithValue()
    {
        var cut = Render<TextAreaField>(p =>
            p.Add(c => c.Label, "Comments")
             .Add(c => c.Value, "Some text here"));

        cut.Find("textarea").TextContent.Should().Contain("Some text here");
    }

    [Fact]
    public void Renders_Disabled()
    {
        var cut = Render<TextAreaField>(p =>
            p.Add(c => c.Label, "Comments")
             .Add(c => c.Disabled, true)
             .Add(c => c.Value, ""));

        cut.Find("textarea").HasAttribute("disabled").Should().BeTrue();
    }

    [Fact]
    public void ValueChanged_FiresOnInput()
    {
        string? newValue = null;

        var cut = Render<TextAreaField>(p =>
            p.Add(c => c.Label, "Comments")
             .Add(c => c.Value, "")
             .Add(c => c.ValueChanged, (string? v) => newValue = v));

        cut.Find("textarea").Input("New content");

        newValue.Should().Be("New content");
    }

    [Fact]
    public void Renders_DefaultRows()
    {
        var cut = Render<TextAreaField>(p =>
            p.Add(c => c.Label, "Comments")
             .Add(c => c.Value, ""));

        cut.Find("textarea").GetAttribute("rows").Should().Be("4");
    }

    [Fact]
    public void Renders_CustomRows()
    {
        var cut = Render<TextAreaField>(p =>
            p.Add(c => c.Label, "Comments")
             .Add(c => c.Value, "")
             .Add(c => c.Rows, 8));

        cut.Find("textarea").GetAttribute("rows").Should().Be("8");
    }

    [Fact]
    public void Renders_Placeholder()
    {
        var cut = Render<TextAreaField>(p =>
            p.Add(c => c.Label, "Comments")
             .Add(c => c.Placeholder, "Enter your feedback")
             .Add(c => c.Value, ""));

        cut.Find("textarea").GetAttribute("placeholder").Should().Be("Enter your feedback");
    }

    [Fact]
    public void Label_LinkedToTextarea_ViaFor()
    {
        var cut = Render<TextAreaField>(p =>
            p.Add(c => c.Label, "Comments")
             .Add(c => c.Value, ""));

        var textareaId = cut.Find("textarea").GetAttribute("id");
        var labelFor = cut.Find("label").GetAttribute("for");
        labelFor.Should().Be(textareaId);
    }
}
