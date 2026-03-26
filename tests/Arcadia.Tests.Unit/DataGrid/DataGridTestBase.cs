using Bunit;
using Microsoft.AspNetCore.Components;
using Arcadia.DataGrid.Components;
using Arcadia.DataGrid.Core;

namespace Arcadia.Tests.Unit.DataGrid;

/// <summary>
/// Shared test model and base class for all DataGrid unit tests.
/// Configures JSInterop in loose mode to avoid failures from
/// the grid's resize-handle interop initialization.
/// </summary>
public record TestEmployee(int Id, string Name, string Department, double Salary, DateTime Hired);

public abstract class DataGridTestBase : BunitContext
{
    protected DataGridTestBase()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    /// <summary>Five sample rows spanning two departments, for most tests.</summary>
    protected static IReadOnlyList<TestEmployee> SampleData =>
        new List<TestEmployee>
        {
            new(1, "Alice",   "Engineering", 120_000, new DateTime(2020, 1, 15)),
            new(2, "Bob",     "Engineering", 110_000, new DateTime(2019, 6, 1)),
            new(3, "Charlie", "Marketing",    95_000, new DateTime(2021, 3, 10)),
            new(4, "Diana",   "Marketing",    98_000, new DateTime(2022, 8, 22)),
            new(5, "Eve",     "Engineering", 130_000, new DateTime(2018, 11, 5)),
        };

    /// <summary>
    /// Larger dataset (30 rows) for paging / grouping tests that need multiple pages.
    /// </summary>
    protected static IReadOnlyList<TestEmployee> LargeData =>
        Enumerable.Range(1, 30).Select(i => new TestEmployee(
            i,
            $"Employee{i}",
            i % 3 == 0 ? "Finance" : i % 2 == 0 ? "Marketing" : "Engineering",
            50_000 + i * 2_000,
            new DateTime(2018, 1, 1).AddDays(i * 30)
        )).ToList();

    /// <summary>
    /// Render a DataGrid and trigger a second render pass so the table appears.
    /// The grid uses a CascadingValue to collect column definitions during
    /// the first render; the table is only produced once Columns.Count > 0
    /// on the second pass.
    /// </summary>
    protected IRenderedComponent<ArcadiaDataGrid<TestEmployee>> RenderDataGrid(
        Action<ComponentParameterCollectionBuilder<ArcadiaDataGrid<TestEmployee>>> builder)
    {
        var cut = Render<ArcadiaDataGrid<TestEmployee>>(builder);
        cut.Render(); // second pass: columns are now collected, table renders
        return cut;
    }

    /// <summary>
    /// Render a grid with the given data and standard Name / Department / Salary columns
    /// using the Property-based reflection accessor.
    /// </summary>
    protected IRenderedComponent<ArcadiaDataGrid<TestEmployee>> RenderGrid(
        IReadOnlyList<TestEmployee> data,
        Action<ComponentParameterCollectionBuilder<ArcadiaDataGrid<TestEmployee>>>? configure = null,
        bool withSalaryColumn = true)
    {
        return RenderDataGrid(p =>
        {
            p.Add(g => g.Data, data);
            p.Add(g => g.PageSize, 0); // no paging by default
            p.AddChildContent<ArcadiaColumn<TestEmployee>>(col =>
                col.Add(c => c.Property, "Name").Add(c => c.Title, "Name"));
            p.AddChildContent<ArcadiaColumn<TestEmployee>>(col =>
                col.Add(c => c.Property, "Department").Add(c => c.Title, "Department"));
            if (withSalaryColumn)
            {
                p.AddChildContent<ArcadiaColumn<TestEmployee>>(col =>
                    col.Add(c => c.Property, "Salary").Add(c => c.Title, "Salary"));
            }
            configure?.Invoke(p);
        });
    }

    /// <summary>
    /// Render a grid with Field-lambda columns instead of Property strings.
    /// </summary>
    protected IRenderedComponent<ArcadiaDataGrid<TestEmployee>> RenderGridWithFieldColumns(
        IReadOnlyList<TestEmployee> data,
        Action<ComponentParameterCollectionBuilder<ArcadiaDataGrid<TestEmployee>>>? configure = null)
    {
        return RenderDataGrid(p =>
        {
            p.Add(g => g.Data, data);
            p.Add(g => g.PageSize, 0);
            p.AddChildContent<ArcadiaColumn<TestEmployee>>(col =>
                col.Add(c => c.Field, (Func<TestEmployee, object>)(e => e.Name))
                   .Add(c => c.Title, "Name"));
            p.AddChildContent<ArcadiaColumn<TestEmployee>>(col =>
                col.Add(c => c.Field, (Func<TestEmployee, object>)(e => e.Department))
                   .Add(c => c.Title, "Department"));
            p.AddChildContent<ArcadiaColumn<TestEmployee>>(col =>
                col.Add(c => c.Field, (Func<TestEmployee, object>)(e => e.Salary))
                   .Add(c => c.Title, "Salary"));
            configure?.Invoke(p);
        });
    }
}
