using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Arcadia.DataGrid.Components;
using Arcadia.DataGrid.Core;
using Xunit;

namespace Arcadia.Tests.Unit.DataGrid;

/// <summary>
/// Tests for inline editing: start edit (double-click), commit (Enter / blur),
/// cancel (Escape), edit callback, and double-commit guard.
/// </summary>
public class DataGridEditTests : DataGridTestBase
{
    private IRenderedComponent<ArcadiaDataGrid<TestEmployee>> RenderEditableGrid(
        EventCallback<TestEmployee>? onRowEdit = null)
    {
        return Render<ArcadiaDataGrid<TestEmployee>>(p =>
        {
            p.Add(g => g.Data, SampleData);
            p.Add(g => g.PageSize, 0);
            if (onRowEdit.HasValue)
                p.Add(g => g.OnRowEdit, onRowEdit.Value);
            p.AddChildContent<ArcadiaColumn<TestEmployee>>(col =>
                col.Add(c => c.Property, "Name")
                   .Add(c => c.Title, "Name")
                   .Add(c => c.Editable, true));
            p.AddChildContent<ArcadiaColumn<TestEmployee>>(col =>
                col.Add(c => c.Property, "Department")
                   .Add(c => c.Title, "Department"));
            p.AddChildContent<ArcadiaColumn<TestEmployee>>(col =>
                col.Add(c => c.Property, "Salary")
                   .Add(c => c.Title, "Salary")
                   .Add(c => c.Editable, true));
        });
    }

    // ── Start editing ──

    [Fact]
    public void DoubleClick_EditableCell_ShowsEditInput()
    {
        var cut = RenderEditableGrid();

        // Double-click the first Name cell
        var nameCell = cut.FindAll("td[role='gridcell']")[0];
        nameCell.DoubleClick();

        cut.FindAll(".arcadia-grid__edit-input").Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public void DoubleClick_NonEditableCell_DoesNotStartEdit()
    {
        var cut = RenderEditableGrid();

        // Double-click the Department cell (not editable)
        var deptCell = cut.FindAll("td[role='gridcell']")[1];
        deptCell.DoubleClick();

        cut.FindAll(".arcadia-grid__edit-input").Count.Should().Be(0);
    }

    [Fact]
    public void StartEdit_AddsEditingCssClass()
    {
        var cut = RenderEditableGrid();

        var nameCell = cut.FindAll("td[role='gridcell']")[0];
        nameCell.DoubleClick();

        cut.FindAll(".arcadia-grid__td--editing").Count.Should().BeGreaterThan(0);
    }

    // ── IsEditing / IsEditingCell ──

    [Fact]
    public void IsEditing_TrueForEditingRow()
    {
        var cut = RenderEditableGrid();

        cut.Instance.StartEdit(SampleData[0], "Name");
        cut.Instance.IsEditing(SampleData[0]).Should().BeTrue();
    }

    [Fact]
    public void IsEditing_FalseForOtherRow()
    {
        var cut = RenderEditableGrid();

        cut.Instance.StartEdit(SampleData[0], "Name");
        cut.Instance.IsEditing(SampleData[1]).Should().BeFalse();
    }

    [Fact]
    public void IsEditingCell_TrueForExactCell()
    {
        var cut = RenderEditableGrid();

        cut.Instance.StartEdit(SampleData[0], "Name");
        cut.Instance.IsEditingCell(SampleData[0], "Name").Should().BeTrue();
    }

    [Fact]
    public void IsEditingCell_FalseForDifferentColumn()
    {
        var cut = RenderEditableGrid();

        cut.Instance.StartEdit(SampleData[0], "Name");
        cut.Instance.IsEditingCell(SampleData[0], "Salary").Should().BeFalse();
    }

    // ── Commit edit ──

    [Fact]
    public async Task CommitEdit_InvokesOnRowEditCallback()
    {
        TestEmployee? editedItem = null;
        var cb = EventCallback.Factory.Create<TestEmployee>(this, e => editedItem = e);
        var cut = RenderEditableGrid(cb);

        cut.Instance.StartEdit(SampleData[0], "Name");
        await cut.Instance.CommitEdit();

        editedItem.Should().NotBeNull();
        editedItem!.Name.Should().Be("Alice");
    }

    [Fact]
    public async Task CommitEdit_ClearsEditState()
    {
        var cb = EventCallback.Factory.Create<TestEmployee>(this, _ => { });
        var cut = RenderEditableGrid(cb);

        cut.Instance.StartEdit(SampleData[0], "Name");
        await cut.Instance.CommitEdit();

        cut.Instance.IsEditing(SampleData[0]).Should().BeFalse();
    }

    // ── Cancel edit ──

    [Fact]
    public void CancelEdit_ClearsEditState()
    {
        var cut = RenderEditableGrid();

        cut.Instance.StartEdit(SampleData[0], "Name");
        cut.Instance.CancelEdit();

        cut.Instance.IsEditing(SampleData[0]).Should().BeFalse();
    }

    [Fact]
    public void CancelEdit_DoesNotFireCallback()
    {
        TestEmployee? editedItem = null;
        var cb = EventCallback.Factory.Create<TestEmployee>(this, e => editedItem = e);
        var cut = RenderEditableGrid(cb);

        cut.Instance.StartEdit(SampleData[0], "Name");
        cut.Instance.CancelEdit();

        editedItem.Should().BeNull();
    }

    // ── Double-commit guard ──

    [Fact]
    public async Task DoubleCommit_GuardPreventsSecondCallback()
    {
        int callCount = 0;
        var cb = EventCallback.Factory.Create<TestEmployee>(this, _ => callCount++);
        var cut = RenderEditableGrid(cb);

        cut.Instance.StartEdit(SampleData[0], "Name");

        // Simulate rapid Enter + blur both calling CommitEdit
        var t1 = cut.Instance.CommitEdit();
        var t2 = cut.Instance.CommitEdit();
        await Task.WhenAll(t1, t2);

        callCount.Should().Be(1);
    }

    // ── Sort clears edit ──

    [Fact]
    public void SortingColumn_CancelsActiveEdit()
    {
        var cut = RenderEditableGrid();

        cut.Instance.StartEdit(SampleData[0], "Name");
        cut.Instance.IsEditing(SampleData[0]).Should().BeTrue();

        // Click a header to sort
        cut.FindAll("th[role='columnheader']")[0].Click();

        cut.Instance.IsEditing(SampleData[0]).Should().BeFalse();
    }
}
