using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Arcadia.DataGrid.Components;
using Arcadia.DataGrid.Core;
using Xunit;

namespace Arcadia.Tests.Unit.DataGrid;

/// <summary>
/// Tests for column pinning (freeze) behavior: IsFrozen defaults, TogglePin,
/// sticky positioning, and column menu state management.
/// </summary>
public class DataGridColumnPinningTests : DataGridTestBase
{
    // ── IsFrozen defaults ──

    [Fact]
    public void IsFrozen_DefaultsToFalse()
    {
        var cut = RenderGrid(SampleData);
        var nameCol = cut.Instance.Collector.Columns.First(c => c.Title == "Name");

        nameCol.IsFrozen.Should().BeFalse();
    }

    [Fact]
    public void IsFrozen_InitializedFromFrozenParameter()
    {
        var cut = RenderDataGrid(p =>
        {
            p.Add(g => g.Data, SampleData);
            p.Add(g => g.PageSize, 0);
            p.AddChildContent<ArcadiaColumn<TestEmployee>>(col =>
                col.Add(c => c.Property, "Name")
                   .Add(c => c.Title, "Name")
                   .Add(c => c.Frozen, true)
                   .Add(c => c.Width, "150px"));
            p.AddChildContent<ArcadiaColumn<TestEmployee>>(col =>
                col.Add(c => c.Property, "Department").Add(c => c.Title, "Department"));
        });

        var nameCol = cut.Instance.Collector.Columns.First(c => c.Title == "Name");
        nameCol.IsFrozen.Should().BeTrue();
    }

    // ── TogglePin ──

    [Fact]
    public void TogglePin_SetsIsFrozenTrue()
    {
        var cut = RenderGrid(SampleData);
        var nameCol = cut.Instance.Collector.Columns.First(c => c.Title == "Name");

        nameCol.IsFrozen.Should().BeFalse("precondition: column starts unfrozen");

        cut.InvokeAsync(() => cut.Instance.TogglePin(nameCol));

        nameCol.IsFrozen.Should().BeTrue();
    }

    [Fact]
    public void TogglePin_CalledTwice_UnfreezsColumn()
    {
        var cut = RenderGrid(SampleData);
        var nameCol = cut.Instance.Collector.Columns.First(c => c.Title == "Name");

        cut.InvokeAsync(() => cut.Instance.TogglePin(nameCol));
        cut.InvokeAsync(() => cut.Instance.TogglePin(nameCol));

        nameCol.IsFrozen.Should().BeFalse();
    }

    // ── Frozen column renders sticky positioning ──

    [Fact]
    public void FrozenColumn_HasStickyPositionStyle()
    {
        var cut = RenderDataGrid(p =>
        {
            p.Add(g => g.Data, SampleData);
            p.Add(g => g.PageSize, 0);
            p.AddChildContent<ArcadiaColumn<TestEmployee>>(col =>
                col.Add(c => c.Property, "Name")
                   .Add(c => c.Title, "Name")
                   .Add(c => c.Frozen, true)
                   .Add(c => c.Width, "150px"));
            p.AddChildContent<ArcadiaColumn<TestEmployee>>(col =>
                col.Add(c => c.Property, "Department").Add(c => c.Title, "Department"));
        });

        var headerCells = cut.FindAll("th[role='columnheader']");
        var nameHeader = headerCells[0]; // Name is first column

        nameHeader.GetAttribute("style").Should().Contain("position:sticky");
        nameHeader.GetAttribute("style").Should().Contain("left:0");
    }

    [Fact]
    public void UnfrozenColumn_DoesNotHaveStickyStyle()
    {
        var cut = RenderGrid(SampleData);

        var headerCells = cut.FindAll("th[role='columnheader']");
        var deptHeader = headerCells[1]; // Department is second column

        var style = deptHeader.GetAttribute("style") ?? "";
        style.Should().NotContain("position:sticky");
    }

    // ── Column menu state ──

    [Fact]
    public void ShowColumnMenu_SetsMenuState()
    {
        var cut = RenderGrid(SampleData);
        var nameCol = cut.Instance.Collector.Columns.First(c => c.Title == "Name");
        var fakeMouseEvent = new MouseEventArgs { ClientX = 100, ClientY = 200 };

        cut.InvokeAsync(() => cut.Instance.ShowColumnMenu(nameCol, fakeMouseEvent));
        cut.Render();

        // The column menu should be visible in the markup
        var menuItems = cut.FindAll(".arcadia-grid__col-menu-item");
        menuItems.Should().NotBeEmpty("column menu should render after ShowColumnMenu");
    }

    [Fact]
    public void CloseColumnMenu_ClearsMenuState()
    {
        var cut = RenderGrid(SampleData);
        var nameCol = cut.Instance.Collector.Columns.First(c => c.Title == "Name");
        var fakeMouseEvent = new MouseEventArgs { ClientX = 100, ClientY = 200 };

        cut.InvokeAsync(() => cut.Instance.ShowColumnMenu(nameCol, fakeMouseEvent));
        cut.Render();
        cut.InvokeAsync(() => cut.Instance.CloseColumnMenu());
        cut.Render();

        // The column menu should no longer be in the markup
        var menuItems = cut.FindAll(".arcadia-grid__col-menu");
        menuItems.Should().BeEmpty("column menu should be hidden after CloseColumnMenu");
    }
}
