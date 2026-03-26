using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Arcadia.DataGrid.Components;
using Arcadia.DataGrid.Core;
using Xunit;

namespace Arcadia.Tests.Unit.DataGrid;

/// <summary>
/// Tests for group rendering, expand/collapse toggling, grouping with sort,
/// and edge cases (no GroupBy, non-existent column).
/// </summary>
public class DataGridGroupingTests : DataGridTestBase
{
    private IRenderedComponent<ArcadiaDataGrid<TestEmployee>> RenderGroupedGrid(
        string groupBy = "Department",
        IReadOnlyList<TestEmployee>? data = null)
    {
        return RenderDataGrid(p =>
        {
            p.Add(g => g.Data, data ?? SampleData);
            p.Add(g => g.PageSize, 0);
            p.Add(g => g.GroupBy, groupBy);
            p.AddChildContent<ArcadiaColumn<TestEmployee>>(col =>
                col.Add(c => c.Property, "Name").Add(c => c.Title, "Name"));
            p.AddChildContent<ArcadiaColumn<TestEmployee>>(col =>
                col.Add(c => c.Property, "Department").Add(c => c.Title, "Department"));
            p.AddChildContent<ArcadiaColumn<TestEmployee>>(col =>
                col.Add(c => c.Property, "Salary").Add(c => c.Title, "Salary"));
        });
    }

    // ── Group rendering ──

    [Fact]
    public void GroupBy_RendersGroupRows()
    {
        var cut = RenderGroupedGrid();

        var groupRows = cut.FindAll(".arcadia-grid__group-row");
        groupRows.Count.Should().Be(2); // Engineering, Marketing
    }

    [Fact]
    public void GroupRow_ShowsGroupLabel()
    {
        var cut = RenderGroupedGrid();

        var groupCells = cut.FindAll(".arcadia-grid__group-cell");
        var labels = groupCells.Select(c => c.TextContent).ToList();
        labels.Should().Contain(l => l.Contains("Engineering"));
        labels.Should().Contain(l => l.Contains("Marketing"));
    }

    [Fact]
    public void GroupRow_ShowsItemCount()
    {
        var cut = RenderGroupedGrid();

        var counts = cut.FindAll(".arcadia-grid__group-count")
            .Select(c => c.TextContent.Trim())
            .ToList();
        // Engineering: 3 (Alice, Bob, Eve), Marketing: 2 (Charlie, Diana)
        counts.Should().Contain("(3)");
        counts.Should().Contain("(2)");
    }

    // ── Expand / collapse ──

    [Fact]
    public void Groups_ExpandedByDefault()
    {
        var cut = RenderGroupedGrid();

        // All data rows visible (3 Engineering + 2 Marketing = 5 data rows)
        var dataRows = cut.FindAll("tr[role='row']");
        // 1 header row + 5 data rows = 6
        dataRows.Count.Should().Be(6);
    }

    [Fact]
    public void CollapseGroup_HidesRows()
    {
        var cut = RenderGroupedGrid();

        // Click the first group row to collapse it
        var groupRows = cut.FindAll(".arcadia-grid__group-row");
        groupRows[0].Click();

        // After collapsing Engineering group, only Marketing rows (2) + header
        var dataRows = cut.FindAll("tr[role='row']");
        dataRows.Count.Should().BeLessThan(6);
    }

    [Fact]
    public void ExpandCollapsedGroup_ShowsRowsAgain()
    {
        var cut = RenderGroupedGrid();

        var groupRows = cut.FindAll(".arcadia-grid__group-row");
        groupRows[0].Click(); // collapse
        groupRows = cut.FindAll(".arcadia-grid__group-row");
        groupRows[0].Click(); // expand

        var dataRows = cut.FindAll("tr[role='row']");
        dataRows.Count.Should().Be(6); // all rows visible again
    }

    [Fact]
    public void CollapseAllGroups_ShowsOnlyGroupHeaders()
    {
        var cut = RenderGroupedGrid();

        var groupRows = cut.FindAll(".arcadia-grid__group-row");
        groupRows[0].Click(); // collapse Engineering
        groupRows = cut.FindAll(".arcadia-grid__group-row");
        groupRows[1].Click(); // collapse Marketing

        var dataRows = cut.FindAll("tr[role='row']");
        // Only the header <tr> has role="row"
        dataRows.Count.Should().Be(1);
    }

    // ── Internal API ──

    [Fact]
    public void IsGroupExpanded_TrueByDefault()
    {
        var cut = RenderGroupedGrid();

        cut.Instance.IsGroupExpanded("Engineering").Should().BeTrue();
        cut.Instance.IsGroupExpanded("Marketing").Should().BeTrue();
    }

    [Fact]
    public void ToggleGroup_SwitchesState()
    {
        var cut = RenderGroupedGrid();

        cut.Instance.ToggleGroup("Engineering");
        cut.Instance.IsGroupExpanded("Engineering").Should().BeFalse();

        cut.Instance.ToggleGroup("Engineering");
        cut.Instance.IsGroupExpanded("Engineering").Should().BeTrue();
    }

    // ── GroupBy with sort ──

    [Fact]
    public void GroupedData_RespectsSortWithinGroups()
    {
        var cut = RenderGroupedGrid();

        // Sort by Name ascending
        cut.FindAll("th[role='columnheader']")[0].Click();

        var groupedData = cut.Instance.GetGroupedData();
        var engGroup = groupedData.First(g => g.Label == "Engineering");
        var engNames = engGroup.Items.Select(e => e.Name).ToList();
        engNames.Should().BeInAscendingOrder();
    }

    // ── Edge cases ──

    [Fact]
    public void NoGroupBy_RendersNormalRows()
    {
        var cut = RenderDataGrid(p =>
        {
            p.Add(g => g.Data, SampleData);
            p.Add(g => g.PageSize, 0);
            p.Add(g => g.GroupBy, (string?)null);
            p.AddChildContent<ArcadiaColumn<TestEmployee>>(col =>
                col.Add(c => c.Property, "Name").Add(c => c.Title, "Name"));
        });

        cut.FindAll(".arcadia-grid__group-row").Count.Should().Be(0);
    }

    [Fact]
    public void GroupBy_InvalidColumn_ReturnsEmptyGroups()
    {
        var cut = RenderGroupedGrid("NonexistentColumn");

        var grouped = cut.Instance.GetGroupedData();
        grouped.Count.Should().Be(0);
    }

    [Fact]
    public void GroupBy_EmptyData_NoGroupRows()
    {
        var cut = RenderGroupedGrid(data: new List<TestEmployee>());

        cut.FindAll(".arcadia-grid__group-row").Count.Should().Be(0);
    }
}
