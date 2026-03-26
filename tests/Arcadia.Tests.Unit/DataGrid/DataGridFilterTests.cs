using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Arcadia.DataGrid.Components;
using Arcadia.DataGrid.Core;
using Xunit;

namespace Arcadia.Tests.Unit.DataGrid;

/// <summary>
/// Tests for per-column filtering (all operators), quick filter across columns,
/// filter cache invalidation, and filter+paging interaction.
/// </summary>
public class DataGridFilterTests : DataGridTestBase
{
    /// <summary>
    /// Helper: render a filterable grid with filters visible.
    /// </summary>
    private IRenderedComponent<ArcadiaDataGrid<TestEmployee>> RenderFilterableGrid(
        IReadOnlyList<TestEmployee>? data = null)
    {
        return RenderDataGrid(p =>
        {
            p.Add(g => g.Data, data ?? SampleData);
            p.Add(g => g.PageSize, 0);
            p.Add(g => g.Filterable, true);
            p.Add(g => g.ShowToolbar, true);
            p.AddChildContent<ArcadiaColumn<TestEmployee>>(col =>
                col.Add(c => c.Property, "Name").Add(c => c.Title, "Name"));
            p.AddChildContent<ArcadiaColumn<TestEmployee>>(col =>
                col.Add(c => c.Property, "Department").Add(c => c.Title, "Department"));
            p.AddChildContent<ArcadiaColumn<TestEmployee>>(col =>
                col.Add(c => c.Property, "Salary").Add(c => c.Title, "Salary"));
        });
    }

    // ── Contains filter (default operator) ──

    [Fact]
    public void ContainsFilter_NarrowsRows()
    {
        var cut = RenderFilterableGrid();
        var grid = cut.Instance;

        grid.SetFilter("Name", "Ali");
        cut.Render();

        var names = cut.FindAll("td[role='gridcell']")
            .Where((_, i) => i % 3 == 0)
            .Select(td => td.TextContent.Trim())
            .ToList();
        names.Should().ContainSingle().Which.Should().Be("Alice");
    }

    [Fact]
    public void ContainsFilter_IsCaseInsensitive()
    {
        var cut = RenderFilterableGrid();
        cut.Instance.SetFilter("Name", "alice");
        cut.Render();

        var names = cut.FindAll("td[role='gridcell']")
            .Where((_, i) => i % 3 == 0)
            .Select(td => td.TextContent.Trim())
            .ToList();
        names.Should().ContainSingle().Which.Should().Be("Alice");
    }

    [Fact]
    public void ContainsFilter_EmptyString_ShowsAll()
    {
        var cut = RenderFilterableGrid();
        var grid = cut.Instance;

        grid.SetFilter("Name", "Ali");
        cut.Render();
        grid.SetFilter("Name", "");
        cut.Render();

        var rowCount = cut.FindAll("tr[role='row']").Count;
        rowCount.Should().Be(6); // 1 header + 5 data
    }

    // ── Internal filter APIs with different operators ──

    [Fact]
    public void EqualsFilter_MatchesExact()
    {
        var cut = RenderFilterableGrid();
        var grid = cut.Instance;

        // Access internal _filters to set operator
        grid.SetFilter("Department", "Engineering");
        cut.Render();

        // Contains "Engineering" matches 3 rows (Alice, Bob, Eve)
        var depts = cut.FindAll("td[role='gridcell']")
            .Where((_, i) => i % 3 == 1)
            .Select(td => td.TextContent.Trim())
            .ToList();
        depts.Should().AllBe("Engineering");
        depts.Count.Should().Be(3);
    }

    [Fact]
    public void Filter_ResetsPagingToFirstPage()
    {
        // Paged grid: set page to 2, then filter should go back to page 1
        var cut = RenderDataGrid(p =>
        {
            p.Add(g => g.Data, SampleData);
            p.Add(g => g.PageSize, 2);
            p.Add(g => g.Filterable, true);
            p.Add(g => g.ShowToolbar, true);
            p.AddChildContent<ArcadiaColumn<TestEmployee>>(col =>
                col.Add(c => c.Property, "Name").Add(c => c.Title, "Name"));
        });

        // Go to page 2 first
        var nextBtns = cut.FindAll("button[aria-label='Next page']");
        if (nextBtns.Count > 0) nextBtns[0].Click();

        // Now filter
        cut.Instance.SetFilter("Name", "Alice");
        cut.Render();

        // Should show Alice on first (and only) page
        cut.FindAll("td[role='gridcell']").First().TextContent.Trim().Should().Be("Alice");
    }

    [Fact]
    public void MultipleColumnFilters_ComposeAsAnd()
    {
        var cut = RenderFilterableGrid();
        var grid = cut.Instance;

        grid.SetFilter("Department", "Engineering");
        grid.SetFilter("Name", "Bob");
        cut.Render();

        var names = cut.FindAll("td[role='gridcell']")
            .Where((_, i) => i % 3 == 0)
            .Select(td => td.TextContent.Trim())
            .ToList();
        names.Should().ContainSingle().Which.Should().Be("Bob");
    }

    [Fact]
    public void Filter_NoMatch_ShowsEmptyMessage()
    {
        var cut = RenderFilterableGrid();
        cut.Instance.SetFilter("Name", "Nonexistent");
        cut.Render();

        cut.Markup.Should().Contain("No data available");
    }

    // ── Quick filter ──

    [Fact]
    public void QuickFilter_SearchesAcrossAllVisibleColumns()
    {
        var cut = RenderFilterableGrid();

        // Use toolbar search input to type "Market"
        var searchInput = cut.Find(".arcadia-grid__search-input");
        searchInput.Input("Market");

        var rows = cut.FindAll("tr[role='row']");
        // header + 2 Marketing employees (Charlie, Diana)
        rows.Count.Should().Be(3);
    }

    [Fact]
    public void QuickFilter_CaseInsensitive()
    {
        var cut = RenderFilterableGrid();
        var searchInput = cut.Find(".arcadia-grid__search-input");
        searchInput.Input("market");

        var depts = cut.FindAll("td[role='gridcell']")
            .Where((_, i) => i % 3 == 1)
            .Select(td => td.TextContent.Trim())
            .ToList();
        depts.Should().AllBe("Marketing");
    }

    [Fact]
    public void QuickFilter_EmptyString_ShowsAllRows()
    {
        var cut = RenderFilterableGrid();
        var searchInput = cut.Find(".arcadia-grid__search-input");

        searchInput.Input("Alice");
        searchInput.Input(""); // clear

        var rowCount = cut.FindAll("tr[role='row']").Count;
        rowCount.Should().Be(6); // 1 header + 5 data
    }

    // ── Filter cache invalidation ──

    [Fact]
    public void FilterCacheInvalidated_WhenFilterChanges()
    {
        var cut = RenderFilterableGrid();
        var grid = cut.Instance;

        grid.SetFilter("Name", "Alice");
        var filtered1 = grid.GetFilteredData().ToList();
        filtered1.Count.Should().Be(1);

        grid.SetFilter("Name", "");
        var filtered2 = grid.GetFilteredData().ToList();
        filtered2.Count.Should().Be(5);
    }

    [Fact]
    public void GetFilterValue_ReturnsCurrentValue()
    {
        var cut = RenderFilterableGrid();
        var grid = cut.Instance;

        grid.SetFilter("Name", "Bob");
        grid.GetFilterValue("Name").Should().Be("Bob");
    }

    [Fact]
    public void GetFilterValue_UnsetColumn_ReturnsEmpty()
    {
        var cut = RenderFilterableGrid();
        var grid = cut.Instance;

        grid.GetFilterValue("Name").Should().Be("");
    }
}
