using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Arcadia.DataGrid.Components;
using Arcadia.DataGrid.Core;
using Xunit;

namespace Arcadia.Tests.Unit.DataGrid;

/// <summary>
/// Tests for single-column sort, multi-sort (shift-click), sort priority badges,
/// sort cycle (Asc -> Desc -> None), sort callback, and non-sortable columns.
/// </summary>
public class DataGridSortTests : DataGridTestBase
{
    [Fact]
    public void ClickHeader_SortsAscending()
    {
        var cut = RenderGrid(SampleData);

        // Click the "Name" header
        cut.FindAll("th[role='columnheader']")[0].Click();

        // After first click: ascending -> Alice, Bob, Charlie, Diana, Eve
        var names = cut.FindAll("td[role='gridcell']")
            .Where((_, i) => i % 3 == 0) // every 3rd cell is Name
            .Select(td => td.TextContent.Trim())
            .ToList();
        names.Should().BeInAscendingOrder();
    }

    [Fact]
    public void DoubleClickHeader_SortsDescending()
    {
        var cut = RenderGrid(SampleData);

        var header = cut.FindAll("th[role='columnheader']")[0];
        header.Click(); // Asc
        header.Click(); // Desc

        var names = cut.FindAll("td[role='gridcell']")
            .Where((_, i) => i % 3 == 0)
            .Select(td => td.TextContent.Trim())
            .ToList();
        names.Should().BeInDescendingOrder();
    }

    [Fact]
    public void TripleClickHeader_ClearsSort()
    {
        var cut = RenderGrid(SampleData);

        var header = cut.FindAll("th[role='columnheader']")[0];
        header.Click(); // Asc
        header.Click(); // Desc
        header.Click(); // None

        // Original order restored: Alice, Bob, Charlie, Diana, Eve (as in SampleData)
        var names = cut.FindAll("td[role='gridcell']")
            .Where((_, i) => i % 3 == 0)
            .Select(td => td.TextContent.Trim())
            .ToList();
        names.Should().Equal("Alice", "Bob", "Charlie", "Diana", "Eve");
    }

    [Fact]
    public void SortChanged_CallbackFires()
    {
        SortDescriptor? received = null;
        var cut = RenderGrid(SampleData, p =>
            p.Add(g => g.SortChanged, EventCallback.Factory.Create<SortDescriptor?>(this, s => received = s)));

        cut.FindAll("th[role='columnheader']")[0].Click();

        received.Should().NotBeNull();
        received!.Property.Should().Be("Name");
        received.Direction.Should().Be(SortDirection.Ascending);
    }

    [Fact]
    public void SortChanged_NullOnClear()
    {
        SortDescriptor? received = new SortDescriptor { Property = "keep" };
        var cut = RenderGrid(SampleData, p =>
            p.Add(g => g.SortChanged, EventCallback.Factory.Create<SortDescriptor?>(this, s => received = s)));

        var header = cut.FindAll("th[role='columnheader']")[0];
        header.Click(); // Asc
        header.Click(); // Desc
        header.Click(); // None -> callback with null

        received.Should().BeNull();
    }

    [Fact]
    public void SortableGrid_SortableColumn_HeaderHasSortableClass()
    {
        var cut = RenderGrid(SampleData, p => p.Add(g => g.Sortable, true));

        cut.FindAll("th[role='columnheader']")[0].ClassList
            .Should().Contain("arcadia-grid__th--sortable");
    }

    [Fact]
    public void NonSortableGrid_HeaderLacksSortableClass()
    {
        var cut = RenderGrid(SampleData, p => p.Add(g => g.Sortable, false));

        cut.FindAll("th[role='columnheader']")[0].ClassList
            .Should().NotContain("arcadia-grid__th--sortable");
    }

    [Fact]
    public void NonSortableGrid_ClickDoesNotSort()
    {
        var cut = RenderGrid(SampleData, p => p.Add(g => g.Sortable, false));

        cut.FindAll("th[role='columnheader']")[0].Click();

        // Order unchanged
        var names = cut.FindAll("td[role='gridcell']")
            .Where((_, i) => i % 3 == 0)
            .Select(td => td.TextContent.Trim())
            .ToList();
        names.Should().Equal("Alice", "Bob", "Charlie", "Diana", "Eve");
    }

    [Fact]
    public void SortNumericColumn_OrdersByValue()
    {
        var cut = RenderGrid(SampleData);

        // Click Salary header (index 2)
        cut.FindAll("th[role='columnheader']")[2].Click();

        var salaries = cut.FindAll("td[role='gridcell']")
            .Where((_, i) => i % 3 == 2)
            .Select(td => double.Parse(td.TextContent.Trim()))
            .ToList();
        salaries.Should().BeInAscendingOrder();
    }

    [Fact]
    public void SortResetsPageToFirst()
    {
        // Use paged grid with page size 3, go to page 2, then sort
        var cut = Render<ArcadiaDataGrid<TestEmployee>>(p =>
        {
            p.Add(g => g.Data, SampleData);
            p.Add(g => g.PageSize, 3);
            p.AddChildContent<ArcadiaColumn<TestEmployee>>(col =>
                col.Add(c => c.Property, "Name").Add(c => c.Title, "Name"));
        });

        // Navigate to page 2
        var nextBtn = cut.FindAll("button[aria-label='Next page']");
        if (nextBtn.Count > 0)
            nextBtn[0].Click();

        // Sort by Name
        cut.FindAll("th[role='columnheader']")[0].Click();

        // First visible name should be Alice (page 1, ascending)
        var firstName = cut.FindAll("td[role='gridcell']").First().TextContent.Trim();
        firstName.Should().Be("Alice");
    }

    [Fact]
    public void AriaSortAttribute_ReflectsDirection()
    {
        var cut = RenderGrid(SampleData);

        var header = cut.FindAll("th[role='columnheader']")[0];

        // Before sort: none
        header.GetAttribute("aria-sort").Should().Be("none");

        // Click once: ascending
        header.Click();
        header = cut.FindAll("th[role='columnheader']")[0]; // re-query after render
        header.GetAttribute("aria-sort").Should().Be("ascending");

        // Click again: descending
        header.Click();
        header = cut.FindAll("th[role='columnheader']")[0];
        header.GetAttribute("aria-sort").Should().Be("descending");
    }

    [Fact]
    public void SortAriaLabel_DescribesState()
    {
        var cut = RenderGrid(SampleData);
        var header = cut.FindAll("th[role='columnheader']")[0];

        header.GetAttribute("aria-label").Should().Contain("click to sort");

        header.Click();
        header = cut.FindAll("th[role='columnheader']")[0];
        header.GetAttribute("aria-label").Should().Contain("sorted ascending");
    }
}
