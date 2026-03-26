using Bunit;
using FluentAssertions;
using Arcadia.FormBuilder.Components.Fields;
using Xunit;

namespace Arcadia.Tests.Unit.FormBuilder;

public class HiddenFieldTests : Arcadia.Tests.Unit.ChartTestBase
{
    [Fact]
    public void Renders_HiddenInput()
    {
        var cut = Render<HiddenField>(p =>
            p.Add(c => c.Name, "token")
             .Add(c => c.Value, "abc123"));

        var input = cut.Find("input[type='hidden']");
        input.Should().NotBeNull();
    }

    [Fact]
    public void Renders_WithValue()
    {
        var cut = Render<HiddenField>(p =>
            p.Add(c => c.Name, "token")
             .Add(c => c.Value, "abc123"));

        cut.Find("input[type='hidden']").GetAttribute("value").Should().Be("abc123");
    }

    [Fact]
    public void Renders_WithName()
    {
        var cut = Render<HiddenField>(p =>
            p.Add(c => c.Name, "csrf_token")
             .Add(c => c.Value, "xyz"));

        cut.Find("input[type='hidden']").GetAttribute("name").Should().Be("csrf_token");
    }

    [Fact]
    public void NoVisibleLabel_Rendered()
    {
        var cut = Render<HiddenField>(p =>
            p.Add(c => c.Name, "token")
             .Add(c => c.Value, "abc123"));

        cut.FindAll("label").Should().BeEmpty();
    }

    [Fact]
    public void Renders_NullValue_AsEmpty()
    {
        var cut = Render<HiddenField>(p =>
            p.Add(c => c.Name, "token")
             .Add(c => c.Value, (string?)null));

        var input = cut.Find("input[type='hidden']");
        input.GetAttribute("value").Should().BeNullOrEmpty();
    }
}
