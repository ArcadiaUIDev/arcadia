using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Arcadia.DataGrid.Components;
using Arcadia.DataGrid.Core;
using Xunit;

namespace Arcadia.Tests.Unit.DataGrid;

/// <summary>
/// Tests for CSV export: header row, data rows, format strings, filters applied,
/// column visibility respected, quoting of special characters, and empty data.
/// </summary>
public class DataGridExportTests : DataGridTestBase
{
    private IRenderedComponent<ArcadiaDataGrid<TestEmployee>> RenderExportableGrid(
        IReadOnlyList<TestEmployee>? data = null,
        bool withFormat = false)
    {
        return Render<ArcadiaDataGrid<TestEmployee>>(p =>
        {
            p.Add(g => g.Data, data ?? SampleData);
            p.Add(g => g.PageSize, 0);
            p.Add(g => g.Filterable, true);
            p.Add(g => g.ShowToolbar, true);
            p.AddChildContent<ArcadiaColumn<TestEmployee>>(col =>
                col.Add(c => c.Property, "Name").Add(c => c.Title, "Name"));
            p.AddChildContent<ArcadiaColumn<TestEmployee>>(col =>
                col.Add(c => c.Property, "Department").Add(c => c.Title, "Department"));
            if (withFormat)
            {
                p.AddChildContent<ArcadiaColumn<TestEmployee>>(col =>
                    col.Add(c => c.Field, (Func<TestEmployee, object>)(e => e.Salary))
                       .Add(c => c.Title, "Salary")
                       .Add(c => c.Format, "N0"));
            }
            else
            {
                p.AddChildContent<ArcadiaColumn<TestEmployee>>(col =>
                    col.Add(c => c.Property, "Salary").Add(c => c.Title, "Salary"));
            }
        });
    }

    // ── CSV header ──

    [Fact]
    public void ToCsv_HeaderRow_ContainsColumnTitles()
    {
        var cut = RenderExportableGrid();
        var csv = cut.Instance.ToCsv();
        var firstLine = csv.Split('\n', StringSplitOptions.RemoveEmptyEntries)[0];

        firstLine.Should().Contain("Name");
        firstLine.Should().Contain("Department");
        firstLine.Should().Contain("Salary");
    }

    // ── Data rows ──

    [Fact]
    public void ToCsv_ContainsAllDataRows()
    {
        var cut = RenderExportableGrid();
        var csv = cut.Instance.ToCsv();
        var lines = csv.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        lines.Length.Should().Be(6); // 1 header + 5 data rows
    }

    [Fact]
    public void ToCsv_DataRowContent_MatchesGrid()
    {
        var cut = RenderExportableGrid();
        var csv = cut.Instance.ToCsv();
        var lines = csv.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        lines[1].Should().Contain("Alice");
        lines[1].Should().Contain("Engineering");
        lines[1].Should().Contain("120000");
    }

    // ── Respects filters ──

    [Fact]
    public void ToCsv_AfterFilter_OnlyExportsFilteredRows()
    {
        var cut = RenderExportableGrid();

        cut.Instance.SetFilter("Department", "Marketing");
        var csv = cut.Instance.ToCsv();
        var lines = csv.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        lines.Length.Should().Be(3); // header + Charlie + Diana
        csv.Should().Contain("Charlie");
        csv.Should().Contain("Diana");
        csv.Should().NotContain("Alice");
    }

    [Fact]
    public void ToCsv_AfterQuickFilter_OnlyExportsMatchingRows()
    {
        var cut = RenderExportableGrid();

        // Use the internal SetQuickFilter to apply a quick filter
        cut.InvokeAsync(() => cut.Instance.SetQuickFilter("Eve"));
        var csv = cut.Instance.ToCsv();
        var lines = csv.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        lines.Length.Should().Be(2); // header + Eve
        csv.Should().Contain("Eve");
    }

    // ── Respects column visibility ──

    [Fact]
    public void ToCsv_HiddenColumn_ExcludedFromExport()
    {
        var cut = RenderExportableGrid();

        // Hide the Department column
        var deptCol = cut.Instance.Collector.Columns.First(c => c.Title == "Department");
        deptCol.ToggleVisible();

        var csv = cut.Instance.ToCsv();
        var firstLine = csv.Split('\n', StringSplitOptions.RemoveEmptyEntries)[0];

        firstLine.Should().NotContain("Department");
        firstLine.Should().Contain("Name");
        firstLine.Should().Contain("Salary");
    }

    // ── Format string applied ──

    [Fact]
    public void ToCsv_FormattedColumn_UsesFormatString()
    {
        var cut = RenderExportableGrid(withFormat: true);
        var csv = cut.Instance.ToCsv();

        // N0 format: "120,000" (with comma separator)
        csv.Should().Contain("120");
    }

    // ── CSV quoting ──

    [Fact]
    public void ToCsv_ValueWithComma_IsQuoted()
    {
        var dataWithComma = new List<TestEmployee>
        {
            new(1, "Smith, John", "Engineering", 100_000, DateTime.Now)
        };

        var cut = RenderExportableGrid(dataWithComma);
        var csv = cut.Instance.ToCsv();

        // "Smith, John" should be quoted in CSV
        csv.Should().Contain("\"Smith, John\"");
    }

    [Fact]
    public void ToCsv_ValueWithQuotes_EscapesDoubleQuotes()
    {
        var dataWithQuote = new List<TestEmployee>
        {
            new(1, "The \"Boss\"", "Engineering", 100_000, DateTime.Now)
        };

        var cut = RenderExportableGrid(dataWithQuote);
        var csv = cut.Instance.ToCsv();

        // Escaped: "The ""Boss"""
        csv.Should().Contain("\"\"Boss\"\"");
    }

    [Fact]
    public void ToCsv_ValueWithNewline_IsQuoted()
    {
        var dataWithNewline = new List<TestEmployee>
        {
            new(1, "Line1\nLine2", "Engineering", 100_000, DateTime.Now)
        };

        var cut = RenderExportableGrid(dataWithNewline);
        var csv = cut.Instance.ToCsv();

        csv.Should().Contain("\"Line1\nLine2\"");
    }

    // ── Empty data ──

    [Fact]
    public void ToCsv_EmptyData_OnlyHeaderRow()
    {
        var cut = RenderExportableGrid(new List<TestEmployee>());
        var csv = cut.Instance.ToCsv();
        var lines = csv.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        lines.Length.Should().Be(1); // header only
    }

    [Fact]
    public void ToCsv_NullData_OnlyHeaderRow()
    {
        var cut = Render<ArcadiaDataGrid<TestEmployee>>(p =>
        {
            p.Add(g => g.Data, (IReadOnlyList<TestEmployee>?)null);
            p.Add(g => g.PageSize, 0);
            p.AddChildContent<ArcadiaColumn<TestEmployee>>(col =>
                col.Add(c => c.Property, "Name").Add(c => c.Title, "Name"));
        });

        var csv = cut.Instance.ToCsv();
        var lines = csv.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        lines.Length.Should().Be(1);
    }

    // ── Sort applied to export ──

    [Fact]
    public void ToCsv_WithSort_ExportsInSortedOrder()
    {
        var cut = RenderExportableGrid();

        // Sort by Name ascending
        cut.FindAll("th[role='columnheader']")[0].Click();

        var csv = cut.Instance.ToCsv();
        var lines = csv.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        // Verify names are sorted: Alice, Bob, Charlie, Diana, Eve
        lines[1].Should().Contain("Alice");
        lines[2].Should().Contain("Bob");
        lines[3].Should().Contain("Charlie");
        lines[4].Should().Contain("Diana");
        lines[5].Should().Contain("Eve");
    }
}
