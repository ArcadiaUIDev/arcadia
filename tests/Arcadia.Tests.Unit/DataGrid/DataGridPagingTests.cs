using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Arcadia.DataGrid.Components;
using Arcadia.DataGrid.Core;
using Xunit;

namespace Arcadia.Tests.Unit.DataGrid;

/// <summary>
/// Tests for pagination: page navigation, page size changes, page count calculation,
/// boundary clamping, and pagination UI visibility.
/// </summary>
public class DataGridPagingTests : DataGridTestBase
{
    private IRenderedComponent<ArcadiaDataGrid<TestEmployee>> RenderPagedGrid(
        int pageSize = 10,
        IReadOnlyList<TestEmployee>? data = null)
    {
        return Render<ArcadiaDataGrid<TestEmployee>>(p =>
        {
            p.Add(g => g.Data, data ?? LargeData);
            p.Add(g => g.PageSize, pageSize);
            p.AddChildContent<ArcadiaColumn<TestEmployee>>(col =>
                col.Add(c => c.Property, "Name").Add(c => c.Title, "Name"));
            p.AddChildContent<ArcadiaColumn<TestEmployee>>(col =>
                col.Add(c => c.Property, "Department").Add(c => c.Title, "Department"));
        });
    }

    // ── 1. Page count ──

    [Theory]
    [InlineData(10, 30, 3)]  // 30 items / 10 = 3 pages
    [InlineData(10, 25, 3)]  // 25 items / 10 = 3 pages (ceil)
    [InlineData(10, 10, 1)]  // exactly one page
    [InlineData(10, 1, 1)]   // single item
    [InlineData(5, 30, 6)]   // 30 / 5 = 6
    public void PageCount_CalculatedCorrectly(int pageSize, int itemCount, int expectedPages)
    {
        var data = Enumerable.Range(1, itemCount)
            .Select(i => new TestEmployee(i, $"E{i}", "Dept", 50000, DateTime.Now))
            .ToList();

        var cut = RenderPagedGrid(pageSize, data);

        // Page info text: "Page 1 of N"
        var pageInfo = cut.Find(".arcadia-grid__page-current");
        pageInfo.TextContent.Should().Contain($"of {expectedPages}");
    }

    // ── 2. First page shows correct rows ──

    [Fact]
    public void FirstPage_ShowsFirstNRows()
    {
        var cut = RenderPagedGrid(10);

        var names = cut.FindAll("td[role='gridcell']")
            .Where((_, i) => i % 2 == 0) // Name column
            .Select(td => td.TextContent.Trim())
            .ToList();
        names.Count.Should().Be(10);
        names.First().Should().Be("Employee1");
        names.Last().Should().Be("Employee10");
    }

    // ── 3. Next page navigation ──

    [Fact]
    public void NextPage_ShowsNextRows()
    {
        var cut = RenderPagedGrid(10);

        cut.Find("button[aria-label='Next page']").Click();

        var names = cut.FindAll("td[role='gridcell']")
            .Where((_, i) => i % 2 == 0)
            .Select(td => td.TextContent.Trim())
            .ToList();
        names.First().Should().Be("Employee11");
        names.Last().Should().Be("Employee20");
    }

    // ── 4. Previous page ──

    [Fact]
    public void PreviousPage_GoesBack()
    {
        var cut = RenderPagedGrid(10);

        cut.Find("button[aria-label='Next page']").Click(); // page 2
        cut.Find("button[aria-label='Previous page']").Click(); // back to page 1

        var firstName = cut.FindAll("td[role='gridcell']")
            .Where((_, i) => i % 2 == 0)
            .First().TextContent.Trim();
        firstName.Should().Be("Employee1");
    }

    // ── 5. Last page ──

    [Fact]
    public void LastPage_ShowsRemainingRows()
    {
        var cut = RenderPagedGrid(10);

        cut.Find("button[aria-label='Last page']").Click();

        var names = cut.FindAll("td[role='gridcell']")
            .Where((_, i) => i % 2 == 0)
            .Select(td => td.TextContent.Trim())
            .ToList();
        names.First().Should().Be("Employee21");
        names.Count.Should().Be(10); // 30 items, page 3 has 10
    }

    // ── 6. First page button ──

    [Fact]
    public void FirstPageButton_ReturnToStart()
    {
        var cut = RenderPagedGrid(10);

        cut.Find("button[aria-label='Last page']").Click(); // go to last
        cut.Find("button[aria-label='First page']").Click(); // back to first

        var firstName = cut.FindAll("td[role='gridcell']")
            .Where((_, i) => i % 2 == 0)
            .First().TextContent.Trim();
        firstName.Should().Be("Employee1");
    }

    // ── 7. Buttons disabled state ──

    [Fact]
    public void FirstPage_PreviousButtonsDisabled()
    {
        var cut = RenderPagedGrid(10);

        cut.Find("button[aria-label='First page']").HasAttribute("disabled").Should().BeTrue();
        cut.Find("button[aria-label='Previous page']").HasAttribute("disabled").Should().BeTrue();
    }

    [Fact]
    public void LastPage_NextButtonsDisabled()
    {
        var cut = RenderPagedGrid(10);

        cut.Find("button[aria-label='Last page']").Click();

        cut.Find("button[aria-label='Next page']").HasAttribute("disabled").Should().BeTrue();
        cut.Find("button[aria-label='Last page']").HasAttribute("disabled").Should().BeTrue();
    }

    // ── 8. Page size change ──

    [Fact]
    public void SetPageSize_ChangesRowCount()
    {
        var cut = RenderPagedGrid(10);
        cut.Instance.SetPageSize(5);
        cut.Render();

        var rowCount = cut.FindAll("td[role='gridcell']")
            .Where((_, i) => i % 2 == 0)
            .Count();
        rowCount.Should().Be(5);
    }

    [Fact]
    public void SetPageSize_ResetsToFirstPage()
    {
        var cut = RenderPagedGrid(10);

        // Go to page 2
        cut.Find("button[aria-label='Next page']").Click();
        // Change page size
        cut.Instance.SetPageSize(5);
        cut.Render();

        var firstName = cut.FindAll("td[role='gridcell']")
            .Where((_, i) => i % 2 == 0)
            .First().TextContent.Trim();
        firstName.Should().Be("Employee1");
    }

    // ── 9. Page info text ──

    [Fact]
    public void PageInfo_ShowsRange()
    {
        var cut = RenderPagedGrid(10);

        var info = cut.Find(".arcadia-grid__page-info").TextContent;
        info.Should().Contain("1");
        info.Should().Contain("10");
        info.Should().Contain("30");
    }

    // ── 10. No pagination when pageSize=0 ──

    [Fact]
    public void PageSizeZero_NoPaginationUI()
    {
        var cut = Render<ArcadiaDataGrid<TestEmployee>>(p =>
        {
            p.Add(g => g.Data, LargeData);
            p.Add(g => g.PageSize, 0);
            p.AddChildContent<ArcadiaColumn<TestEmployee>>(col =>
                col.Add(c => c.Property, "Name").Add(c => c.Title, "Name"));
        });

        cut.FindAll(".arcadia-grid__pagination").Count.Should().Be(0);
    }

    [Fact]
    public void PageSizeZero_ShowsAllRows()
    {
        var cut = Render<ArcadiaDataGrid<TestEmployee>>(p =>
        {
            p.Add(g => g.Data, LargeData);
            p.Add(g => g.PageSize, 0);
            p.AddChildContent<ArcadiaColumn<TestEmployee>>(col =>
                col.Add(c => c.Property, "Name").Add(c => c.Title, "Name"));
        });

        var rowCount = cut.FindAll("td[role='gridcell']").Count;
        rowCount.Should().Be(30);
    }

    // ── 11. GoToPage boundary clamping ──

    [Fact]
    public void GoToPage_NegativeIndex_ClampsToZero()
    {
        var cut = RenderPagedGrid(10);
        cut.Instance.GoToPage(-5);
        cut.Render();

        var firstName = cut.FindAll("td[role='gridcell']")
            .Where((_, i) => i % 2 == 0)
            .First().TextContent.Trim();
        firstName.Should().Be("Employee1");
    }

    [Fact]
    public void GoToPage_BeyondLastPage_ClampsToLast()
    {
        var cut = RenderPagedGrid(10);
        cut.Instance.GoToPage(100);
        cut.Render();

        var names = cut.FindAll("td[role='gridcell']")
            .Where((_, i) => i % 2 == 0)
            .Select(td => td.TextContent.Trim())
            .ToList();
        names.First().Should().Be("Employee21"); // last page
    }
}
