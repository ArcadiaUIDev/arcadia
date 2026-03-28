using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Arcadia.DataGrid.Components;
using Arcadia.DataGrid.Core;
using Xunit;

namespace Arcadia.Tests.Unit.DataGrid;

/// <summary>
/// Tests for automatic column data-type detection and boolean filter behavior.
/// </summary>
public class DataGridTypedFilterTests : DataGridTestBase
{
    // Test model with varied property types for detection
    private record TypedModel(
        string Name,
        int Count,
        double Price,
        bool Active,
        DateTime Created,
        bool? OptionalFlag,
        DateTime? OptionalDate);

    private static readonly IReadOnlyList<TypedModel> TypedData = new List<TypedModel>
    {
        new("Alpha", 10, 1.5, true,  new DateTime(2024, 1, 1), true,  new DateTime(2024, 1, 1)),
        new("Beta",  20, 2.5, false, new DateTime(2024, 2, 1), false, new DateTime(2024, 2, 1)),
        new("Gamma", 30, 3.5, true,  new DateTime(2024, 3, 1), null,  null),
    };

    private IRenderedComponent<ArcadiaDataGrid<TypedModel>> RenderTypedGrid()
    {
        var cut = Render<ArcadiaDataGrid<TypedModel>>(p =>
        {
            p.Add(g => g.Data, TypedData);
            p.Add(g => g.PageSize, 0);
            p.Add(g => g.Filterable, true);
            p.AddChildContent<ArcadiaColumn<TypedModel>>(col =>
                col.Add(c => c.Property, "Name").Add(c => c.Title, "Name"));
            p.AddChildContent<ArcadiaColumn<TypedModel>>(col =>
                col.Add(c => c.Property, "Count").Add(c => c.Title, "Count"));
            p.AddChildContent<ArcadiaColumn<TypedModel>>(col =>
                col.Add(c => c.Property, "Price").Add(c => c.Title, "Price"));
            p.AddChildContent<ArcadiaColumn<TypedModel>>(col =>
                col.Add(c => c.Property, "Active").Add(c => c.Title, "Active"));
            p.AddChildContent<ArcadiaColumn<TypedModel>>(col =>
                col.Add(c => c.Property, "Created").Add(c => c.Title, "Created"));
            p.AddChildContent<ArcadiaColumn<TypedModel>>(col =>
                col.Add(c => c.Property, "OptionalFlag").Add(c => c.Title, "OptionalFlag"));
            p.AddChildContent<ArcadiaColumn<TypedModel>>(col =>
                col.Add(c => c.Property, "OptionalDate").Add(c => c.Title, "OptionalDate"));
        });
        cut.Render();
        return cut;
    }

    // ── Data type detection ──

    [Fact]
    public void BooleanProperty_DetectedAsBooleanType()
    {
        var cut = RenderTypedGrid();
        var col = cut.Instance.Collector.Columns.First(c => c.Title == "Active");

        col.DetectedDataType.Should().Be(ColumnDataType.Boolean);
    }

    [Fact]
    public void DateTimeProperty_DetectedAsDateType()
    {
        var cut = RenderTypedGrid();
        var col = cut.Instance.Collector.Columns.First(c => c.Title == "Created");

        col.DetectedDataType.Should().Be(ColumnDataType.Date);
    }

    [Theory]
    [InlineData("Count")]
    [InlineData("Price")]
    public void NumericProperty_DetectedAsNumberType(string columnTitle)
    {
        var cut = RenderTypedGrid();
        var col = cut.Instance.Collector.Columns.First(c => c.Title == columnTitle);

        col.DetectedDataType.Should().Be(ColumnDataType.Number);
    }

    [Fact]
    public void StringProperty_DetectedAsStringType()
    {
        var cut = RenderTypedGrid();
        var col = cut.Instance.Collector.Columns.First(c => c.Title == "Name");

        col.DetectedDataType.Should().Be(ColumnDataType.String);
    }

    [Fact]
    public void NullableBool_DetectedAsBooleanType()
    {
        var cut = RenderTypedGrid();
        var col = cut.Instance.Collector.Columns.First(c => c.Title == "OptionalFlag");

        col.DetectedDataType.Should().Be(ColumnDataType.Boolean);
    }

    [Fact]
    public void NullableDateTime_DetectedAsDateType()
    {
        var cut = RenderTypedGrid();
        var col = cut.Instance.Collector.Columns.First(c => c.Title == "OptionalDate");

        col.DetectedDataType.Should().Be(ColumnDataType.Date);
    }

    // ── Boolean filter behavior ──

    [Fact]
    public void SetBooleanFilter_WithTrue_FiltersToTrueValues()
    {
        // Use TestEmployee data with the main grid since SetBooleanFilter is on ArcadiaDataGrid<T>
        // We need a model with a bool property. Use the TypedModel grid approach.
        var cut = RenderTypedGrid();

        cut.InvokeAsync(() => cut.Instance.SetBooleanFilter("Active", "True"));
        cut.Render();

        // After filtering, only rows with Active=true should appear
        var rows = cut.FindAll("tbody tr[role='row']");
        rows.Count.Should().Be(2); // Alpha and Gamma have Active=true
    }

    [Fact]
    public void SetBooleanFilter_WithEmptyString_ClearsFilter()
    {
        var cut = RenderTypedGrid();

        // First apply filter
        cut.InvokeAsync(() => cut.Instance.SetBooleanFilter("Active", "True"));
        cut.Render();
        cut.FindAll("tbody tr[role='row']").Count.Should().Be(2, "precondition: filter applied");

        // Then clear it
        cut.InvokeAsync(() => cut.Instance.SetBooleanFilter("Active", ""));
        cut.Render();

        var rows = cut.FindAll("tbody tr[role='row']");
        rows.Count.Should().Be(3, "all rows should show after clearing filter");
    }
}
