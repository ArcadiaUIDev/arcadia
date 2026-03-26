using Bunit;
using FluentAssertions;
using Arcadia.FormBuilder.Components.Fields;
using Microsoft.AspNetCore.Components;
using Xunit;

namespace Arcadia.Tests.Unit.FormBuilder;

public class RepeaterFieldTests : Arcadia.Tests.Unit.ChartTestBase
{
    [Fact]
    public void Renders_WithLegend()
    {
        var cut = Render<RepeaterField>(p =>
            p.Add(c => c.Label, "Line Items")
             .Add(c => c.Rows, new List<Dictionary<string, object?>>()));

        cut.Find("legend").TextContent.Should().Contain("Line Items");
    }

    [Fact]
    public void Renders_CorrectNumberOfRows()
    {
        var rows = new List<Dictionary<string, object?>>
        {
            new() { ["name"] = "Item 1" },
            new() { ["name"] = "Item 2" },
            new() { ["name"] = "Item 3" }
        };

        var cut = Render<RepeaterField>(p =>
            p.Add(c => c.Label, "Line Items")
             .Add(c => c.Rows, rows));

        cut.FindAll(".arcadia-repeater__row").Should().HaveCount(3);
    }

    [Fact]
    public void Renders_Disabled()
    {
        var cut = Render<RepeaterField>(p =>
            p.Add(c => c.Label, "Line Items")
             .Add(c => c.Disabled, true)
             .Add(c => c.Rows, new List<Dictionary<string, object?>>()));

        cut.Find(".arcadia-repeater__add").HasAttribute("disabled").Should().BeTrue();
    }

    [Fact]
    public void AddButton_AddsRow()
    {
        List<Dictionary<string, object?>>? newRows = null;
        var rows = new List<Dictionary<string, object?>>();

        var cut = Render<RepeaterField>(p =>
            p.Add(c => c.Label, "Line Items")
             .Add(c => c.Rows, rows)
             .Add(c => c.RowsChanged, (List<Dictionary<string, object?>> r) => newRows = r));

        cut.Find(".arcadia-repeater__add").Click();

        newRows.Should().NotBeNull();
        newRows.Should().HaveCount(1);
    }

    [Fact]
    public void RemoveButton_RemovesRow()
    {
        List<Dictionary<string, object?>>? newRows = null;
        var rows = new List<Dictionary<string, object?>>
        {
            new() { ["name"] = "Item 1" },
            new() { ["name"] = "Item 2" }
        };

        var cut = Render<RepeaterField>(p =>
            p.Add(c => c.Label, "Line Items")
             .Add(c => c.Rows, rows)
             .Add(c => c.RowsChanged, (List<Dictionary<string, object?>> r) => newRows = r));

        cut.FindAll(".arcadia-repeater__remove")[0].Click();

        newRows.Should().NotBeNull();
        newRows.Should().HaveCount(1);
    }

    [Fact]
    public void RemoveButton_Disabled_AtMinRows()
    {
        var rows = new List<Dictionary<string, object?>>
        {
            new() { ["name"] = "Item 1" }
        };

        var cut = Render<RepeaterField>(p =>
            p.Add(c => c.Label, "Line Items")
             .Add(c => c.Rows, rows)
             .Add(c => c.MinRows, 1));

        cut.Find(".arcadia-repeater__remove").HasAttribute("disabled").Should().BeTrue();
    }

    [Fact]
    public void AddButton_Disabled_AtMaxRows()
    {
        var rows = new List<Dictionary<string, object?>>
        {
            new() { ["name"] = "Item 1" },
            new() { ["name"] = "Item 2" }
        };

        var cut = Render<RepeaterField>(p =>
            p.Add(c => c.Label, "Line Items")
             .Add(c => c.Rows, rows)
             .Add(c => c.MaxRows, 2));

        cut.Find(".arcadia-repeater__add").HasAttribute("disabled").Should().BeTrue();
    }

    [Fact]
    public void AddButton_CustomText()
    {
        var cut = Render<RepeaterField>(p =>
            p.Add(c => c.Label, "Contacts")
             .Add(c => c.Rows, new List<Dictionary<string, object?>>())
             .Add(c => c.AddText, "+ Add Contact"));

        cut.Find(".arcadia-repeater__add").TextContent.Trim().Should().Be("+ Add Contact");
    }
}
