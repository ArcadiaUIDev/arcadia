using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Arcadia.DataGrid.Components;
using Arcadia.DataGrid.Core;
using Xunit;

namespace Arcadia.Tests.Unit.DataGrid;

/// <summary>
/// Tests for single-select, multi-select, select-all, deselect,
/// SelectionMode enum mapping, and SelectedItemsChanged callback.
/// </summary>
public class DataGridSelectionTests : DataGridTestBase
{
    // ── Single select ──

    [Fact]
    public void SingleSelect_ClickRow_SelectsIt()
    {
        var cut = RenderGrid(SampleData, p =>
        {
            p.Add(g => g.SelectionMode, DataGridSelectionMode.SingleRow);
        });

        var rows = cut.FindAll("tr[role='row']");
        rows[1].Click(); // first data row

        rows = cut.FindAll("tr[role='row']");
        rows[1].ClassList.Should().Contain("arcadia-grid__row--selected");
    }

    [Fact]
    public void SingleSelect_ClickAnother_DeselectsPrevious()
    {
        var cut = RenderGrid(SampleData, p =>
        {
            p.Add(g => g.SelectionMode, DataGridSelectionMode.SingleRow);
        });

        var rows = cut.FindAll("tr[role='row']");
        rows[1].Click(); // select Alice
        rows = cut.FindAll("tr[role='row']");
        rows[2].Click(); // select Bob

        rows = cut.FindAll("tr[role='row']");
        rows[1].ClassList.Should().NotContain("arcadia-grid__row--selected");
        rows[2].ClassList.Should().Contain("arcadia-grid__row--selected");
    }

    [Fact]
    public void SingleSelect_SelectedItemsChanged_FiresWithOneItem()
    {
        HashSet<TestEmployee>? received = null;
        var cut = RenderGrid(SampleData, p =>
        {
            p.Add(g => g.SelectionMode, DataGridSelectionMode.SingleRow);
            p.Add(g => g.SelectedItemsChanged,
                EventCallback.Factory.Create<HashSet<TestEmployee>>(this, s => received = s));
        });

        cut.FindAll("tr[role='row']")[1].Click();

        received.Should().NotBeNull();
        received!.Count.Should().Be(1);
        received.First().Name.Should().Be("Alice");
    }

    // ── Multi select ──

    [Fact]
    public void MultiSelect_CheckboxColumnRendered()
    {
        var cut = RenderGrid(SampleData, p =>
        {
            p.Add(g => g.SelectionMode, DataGridSelectionMode.Multiple);
        });

        cut.FindAll("th.arcadia-grid__th--checkbox").Count.Should().Be(1);
    }

    [Fact]
    public void MultiSelect_ClickCheckbox_SelectsRow()
    {
        var cut = RenderGrid(SampleData, p =>
        {
            p.Add(g => g.SelectionMode, DataGridSelectionMode.Multiple);
        });

        // Click the first row's checkbox
        var checkboxes = cut.FindAll("td.arcadia-grid__td--checkbox input[type='checkbox']");
        checkboxes[0].Click();

        cut.Instance.IsSelected(SampleData[0]).Should().BeTrue();
    }

    [Fact]
    public void MultiSelect_ClickMultipleCheckboxes_SelectsMultiple()
    {
        HashSet<TestEmployee>? received = null;
        var cut = RenderGrid(SampleData, p =>
        {
            p.Add(g => g.SelectionMode, DataGridSelectionMode.Multiple);
            p.Add(g => g.SelectedItemsChanged,
                EventCallback.Factory.Create<HashSet<TestEmployee>>(this, s => received = s));
        });

        var checkboxes = cut.FindAll("td.arcadia-grid__td--checkbox input[type='checkbox']");
        checkboxes[0].Click(); // Alice
        checkboxes = cut.FindAll("td.arcadia-grid__td--checkbox input[type='checkbox']");
        checkboxes[1].Click(); // Bob

        received.Should().NotBeNull();
        received!.Count.Should().Be(2);
    }

    [Fact]
    public void MultiSelect_DeselectCheckbox_RemovesFromSelection()
    {
        var cut = RenderGrid(SampleData, p =>
        {
            p.Add(g => g.SelectionMode, DataGridSelectionMode.Multiple);
        });

        var checkboxes = cut.FindAll("td.arcadia-grid__td--checkbox input[type='checkbox']");
        checkboxes[0].Click(); // select Alice
        cut.Instance.IsSelected(SampleData[0]).Should().BeTrue();

        checkboxes = cut.FindAll("td.arcadia-grid__td--checkbox input[type='checkbox']");
        checkboxes[0].Click(); // deselect Alice
        cut.Instance.IsSelected(SampleData[0]).Should().BeFalse();
    }

    // ── Select All ──

    [Fact]
    public void SelectAll_ChecksAllVisibleRows()
    {
        var cut = RenderGrid(SampleData, p =>
        {
            p.Add(g => g.SelectionMode, DataGridSelectionMode.Multiple);
        });

        // Click the select-all checkbox in the header
        var headerCheckbox = cut.Find("th.arcadia-grid__th--checkbox input[type='checkbox']");
        headerCheckbox.Click();

        cut.Instance.IsAllSelected().Should().BeTrue();
        foreach (var item in SampleData)
        {
            cut.Instance.IsSelected(item).Should().BeTrue();
        }
    }

    [Fact]
    public void DeselectAll_UnchecksAllRows()
    {
        var cut = RenderGrid(SampleData, p =>
        {
            p.Add(g => g.SelectionMode, DataGridSelectionMode.Multiple);
        });

        var headerCheckbox = cut.Find("th.arcadia-grid__th--checkbox input[type='checkbox']");
        headerCheckbox.Click(); // select all
        headerCheckbox = cut.Find("th.arcadia-grid__th--checkbox input[type='checkbox']");
        headerCheckbox.Click(); // deselect all

        cut.Instance.IsAllSelected().Should().BeFalse();
    }

    // ── SelectionMode enum ──

    [Fact]
    public void SelectionModeNone_ClickDoesNotSelect()
    {
        var cut = RenderGrid(SampleData, p =>
        {
            p.Add(g => g.SelectionMode, DataGridSelectionMode.None);
        });

        cut.FindAll("tr[role='row']")[1].Click();

        cut.FindAll("tr.arcadia-grid__row--selected").Count.Should().Be(0);
    }

    [Fact]
    public void SelectionModeNone_NoCheckboxColumn()
    {
        var cut = RenderGrid(SampleData, p =>
        {
            p.Add(g => g.SelectionMode, DataGridSelectionMode.None);
        });

        cut.FindAll("th.arcadia-grid__th--checkbox").Count.Should().Be(0);
    }

    [Fact]
    public void SelectionModeSingleRow_SetsSelectableTrue()
    {
        var cut = RenderGrid(SampleData, p =>
        {
            p.Add(g => g.SelectionMode, DataGridSelectionMode.SingleRow);
        });

        // Clicking a row should add the selected class
        cut.FindAll("tr[role='row']")[1].Click();
        cut.FindAll("tr[role='row']")[1].ClassList.Should().Contain("arcadia-grid__row--selected");
    }

    [Fact]
    public void SelectionModeMultiple_SetsMultiSelectTrue()
    {
        var cut = RenderGrid(SampleData, p =>
        {
            p.Add(g => g.SelectionMode, DataGridSelectionMode.Multiple);
        });

        cut.FindAll("th.arcadia-grid__th--checkbox").Count.Should().Be(1);
    }

    // ── Edge: empty data ──

    [Fact]
    public void IsAllSelected_EmptyData_ReturnsFalse()
    {
        var cut = RenderGrid(new List<TestEmployee>(), p =>
        {
            p.Add(g => g.SelectionMode, DataGridSelectionMode.Multiple);
        });

        cut.Instance.IsAllSelected().Should().BeFalse();
    }
}
