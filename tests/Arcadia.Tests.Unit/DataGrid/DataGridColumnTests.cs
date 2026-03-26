using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Arcadia.DataGrid.Components;
using Arcadia.DataGrid.Core;
using Xunit;

namespace Arcadia.Tests.Unit.DataGrid;

/// <summary>
/// Tests for ArcadiaColumn: Property reflection, Field lambda, ResolvedKey,
/// Format strings, visibility toggle, frozen columns, alignment, and width.
/// </summary>
public class DataGridColumnTests : DataGridTestBase
{
    // ── Property-based accessor (reflection) ──

    [Fact]
    public void PropertyParam_ResolvesViaReflection()
    {
        var cut = RenderGrid(SampleData);

        // Verify the Name column resolved correctly by checking cell content
        var firstCell = cut.FindAll("td[role='gridcell']")[0];
        firstCell.TextContent.Trim().Should().Be("Alice");
    }

    [Fact]
    public void PropertyParam_CaseInsensitive()
    {
        var cut = Render<ArcadiaDataGrid<TestEmployee>>(p =>
        {
            p.Add(g => g.Data, SampleData);
            p.Add(g => g.PageSize, 0);
            p.AddChildContent<ArcadiaColumn<TestEmployee>>(col =>
                col.Add(c => c.Property, "name") // lowercase
                   .Add(c => c.Title, "Name"));
        });

        var firstCell = cut.FindAll("td[role='gridcell']")[0];
        firstCell.TextContent.Trim().Should().Be("Alice");
    }

    [Fact]
    public void PropertyParam_AutoGeneratesTitle()
    {
        var cut = Render<ArcadiaDataGrid<TestEmployee>>(p =>
        {
            p.Add(g => g.Data, SampleData);
            p.Add(g => g.PageSize, 0);
            p.AddChildContent<ArcadiaColumn<TestEmployee>>(col =>
                col.Add(c => c.Property, "Department")); // no Title set
        });

        var header = cut.Find("th[role='columnheader']");
        header.TextContent.Should().Contain("Department");
    }

    // ── Field-based accessor ──

    [Fact]
    public void FieldParam_UsesLambdaDirectly()
    {
        var cut = RenderGridWithFieldColumns(SampleData);

        var firstCell = cut.FindAll("td[role='gridcell']")[0];
        firstCell.TextContent.Trim().Should().Be("Alice");
    }

    [Fact]
    public void FieldParam_OverridesProperty()
    {
        // When both Field and Property are set, Field takes precedence
        var cut = Render<ArcadiaDataGrid<TestEmployee>>(p =>
        {
            p.Add(g => g.Data, SampleData);
            p.Add(g => g.PageSize, 0);
            p.AddChildContent<ArcadiaColumn<TestEmployee>>(col =>
                col.Add(c => c.Property, "Name")
                   .Add(c => c.Field, (Func<TestEmployee, object>)(e => e.Department))
                   .Add(c => c.Title, "Test"));
        });

        // Cell should show Department value (from Field) not Name (from Property)
        var firstCell = cut.FindAll("td[role='gridcell']")[0];
        firstCell.TextContent.Trim().Should().Be("Engineering");
    }

    // ── ResolvedKey ──

    [Fact]
    public void ResolvedKey_UsesExplicitKey()
    {
        var cut = Render<ArcadiaDataGrid<TestEmployee>>(p =>
        {
            p.Add(g => g.Data, SampleData);
            p.Add(g => g.PageSize, 0);
            p.AddChildContent<ArcadiaColumn<TestEmployee>>(col =>
                col.Add(c => c.Property, "Name")
                   .Add(c => c.Key, "emp_name")
                   .Add(c => c.Title, "Name"));
        });

        var collector = cut.Instance.Collector;
        collector.Columns[0].ResolvedKey.Should().Be("emp_name");
    }

    [Fact]
    public void ResolvedKey_FallsBackToProperty()
    {
        var cut = Render<ArcadiaDataGrid<TestEmployee>>(p =>
        {
            p.Add(g => g.Data, SampleData);
            p.Add(g => g.PageSize, 0);
            p.AddChildContent<ArcadiaColumn<TestEmployee>>(col =>
                col.Add(c => c.Property, "Name")
                   .Add(c => c.Title, "Name"));
        });

        var collector = cut.Instance.Collector;
        collector.Columns[0].ResolvedKey.Should().Be("Name");
    }

    [Fact]
    public void ResolvedKey_FallsBackToTitle()
    {
        var cut = Render<ArcadiaDataGrid<TestEmployee>>(p =>
        {
            p.Add(g => g.Data, SampleData);
            p.Add(g => g.PageSize, 0);
            p.AddChildContent<ArcadiaColumn<TestEmployee>>(col =>
                col.Add(c => c.Field, (Func<TestEmployee, object>)(e => e.Name))
                   .Add(c => c.Title, "Employee Name"));
        });

        var collector = cut.Instance.Collector;
        collector.Columns[0].ResolvedKey.Should().Be("Employee Name");
    }

    // ── Format string ──

    [Fact]
    public void Format_AppliedToNumericValues()
    {
        var cut = Render<ArcadiaDataGrid<TestEmployee>>(p =>
        {
            p.Add(g => g.Data, SampleData);
            p.Add(g => g.PageSize, 0);
            p.AddChildContent<ArcadiaColumn<TestEmployee>>(col =>
                col.Add(c => c.Field, (Func<TestEmployee, object>)(e => e.Salary))
                   .Add(c => c.Title, "Salary")
                   .Add(c => c.Format, "N0"));
        });

        var firstSalary = cut.FindAll("td[role='gridcell']")[0].TextContent.Trim();
        // N0 format: "120,000" (with thousands separator)
        firstSalary.Should().Contain(",");
    }

    [Fact]
    public void FormatValue_NullValue_ReturnsEmpty()
    {
        var col = new ArcadiaColumn<TestEmployee>();
        col.FormatValue(null).Should().Be("");
    }

    [Fact]
    public void FormatValue_NoFormat_ReturnsToString()
    {
        var col = new ArcadiaColumn<TestEmployee>();
        col.FormatValue(42).Should().Be("42");
    }

    // ── Visibility ──

    [Fact]
    public void HiddenColumn_NotRendered()
    {
        var cut = Render<ArcadiaDataGrid<TestEmployee>>(p =>
        {
            p.Add(g => g.Data, SampleData);
            p.Add(g => g.PageSize, 0);
            p.AddChildContent<ArcadiaColumn<TestEmployee>>(col =>
                col.Add(c => c.Property, "Name").Add(c => c.Title, "Name"));
            p.AddChildContent<ArcadiaColumn<TestEmployee>>(col =>
                col.Add(c => c.Property, "Department")
                   .Add(c => c.Title, "Department")
                   .Add(c => c.Visible, false));
        });

        var headers = cut.FindAll("th[role='columnheader']");
        headers.Count.Should().Be(1);
        headers[0].TextContent.Should().Contain("Name");
    }

    [Fact]
    public void ToggleVisible_FlipsIsVisible()
    {
        var cut = RenderGrid(SampleData);
        var col = cut.Instance.Collector.Columns[0];

        col.IsVisible.Should().BeTrue();
        col.ToggleVisible();
        col.IsVisible.Should().BeFalse();
        col.ToggleVisible();
        col.IsVisible.Should().BeTrue();
    }

    // ── Frozen column ──

    [Fact]
    public void FrozenColumn_HasStickyStyle()
    {
        var cut = Render<ArcadiaDataGrid<TestEmployee>>(p =>
        {
            p.Add(g => g.Data, SampleData);
            p.Add(g => g.PageSize, 0);
            p.AddChildContent<ArcadiaColumn<TestEmployee>>(col =>
                col.Add(c => c.Property, "Name")
                   .Add(c => c.Title, "Name")
                   .Add(c => c.Frozen, true)
                   .Add(c => c.Width, "200px"));
            p.AddChildContent<ArcadiaColumn<TestEmployee>>(col =>
                col.Add(c => c.Property, "Department")
                   .Add(c => c.Title, "Department"));
        });

        var header = cut.FindAll("th[role='columnheader']")[0];
        header.GetAttribute("style").Should().Contain("position:sticky");
    }

    // ── Width ──

    [Fact]
    public void Width_SetsHeaderStyle()
    {
        var cut = Render<ArcadiaDataGrid<TestEmployee>>(p =>
        {
            p.Add(g => g.Data, SampleData);
            p.Add(g => g.PageSize, 0);
            p.AddChildContent<ArcadiaColumn<TestEmployee>>(col =>
                col.Add(c => c.Property, "Name")
                   .Add(c => c.Title, "Name")
                   .Add(c => c.Width, "150px"));
        });

        var header = cut.Find("th[role='columnheader']");
        header.GetAttribute("style").Should().Contain("width:150px");
    }

    // ── Alignment ──

    [Fact]
    public void Align_SetsTextAlignment()
    {
        var cut = Render<ArcadiaDataGrid<TestEmployee>>(p =>
        {
            p.Add(g => g.Data, SampleData);
            p.Add(g => g.PageSize, 0);
            p.AddChildContent<ArcadiaColumn<TestEmployee>>(col =>
                col.Add(c => c.Property, "Salary")
                   .Add(c => c.Title, "Salary")
                   .Add(c => c.Align, "right"));
        });

        var cell = cut.Find("td[role='gridcell']");
        cell.GetAttribute("style").Should().Contain("text-align:right");
    }
}
