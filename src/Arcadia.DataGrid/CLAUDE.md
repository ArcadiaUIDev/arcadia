# CLAUDE.md — Arcadia.DataGrid

## Role: High-performance DataGrid

Pure-C# Blazor DataGrid. **No AG Grid dependency.** Renders rows via Blazor's
`<Virtualize>` for large datasets and a hand-written `datagrid-interop.js`
module for column resize and infinite scroll observation.

## Actual Architecture

```
src/Arcadia.DataGrid/
├── Components/
│   ├── ArcadiaDataGrid.razor           # Generic-typed grid<TItem>
│   ├── ArcadiaDataGrid.razor.cs        # Code-behind (all parameters + logic)
│   ├── ArcadiaColumn.razor             # Column definition (Property, Header, Template, etc.)
│   └── ArcadiaColumn.razor.cs
├── Core/
│   ├── DataGridSelectionMode.cs        # None | Single | Multiple
│   ├── DataGridState.cs                # Persisted state (sort, filter, columns, page size)
│   ├── SortDescriptor.cs
│   ├── FilterDescriptor.cs
│   ├── BatchEditChange.cs
│   └── CollectionObserver.cs           # INotifyCollectionChanged subscription helper
├── wwwroot/
│   ├── js/
│   │   └── datagrid-interop.js         # Column resize, infinite-scroll IntersectionObserver
│   └── css/
│       └── arcadia-datagrid.css        # Grid styles via theme tokens
└── Arcadia.DataGrid.csproj             # Multi-targets net5.0..net10.0
```

The grid is `ArcadiaDataGrid<TItem> : ArcadiaComponentBase, IAsyncDisposable`.
Columns are defined as `<ArcadiaColumn>` children inside `<ArcadiaDataGrid>`.

## Performance Targets

- Initial render: < 100ms for 1,000 rows
- Sort / filter: < 50ms for 10,000 rows (client-side)
- Virtualized scroll: 60fps
- JS interop calls per user action: ≤ 3
- Memory: no growth on repeated sort/filter cycles

## JS Interop Rules

1. **Isolate** — use `IJSObjectReference` imported from
   `./_content/Arcadia.DataGrid/js/datagrid-interop.js`.
2. **Batch** — collect multiple operations into a single interop call where possible.
3. **Dispose** — implement `IAsyncDisposable`; dispose every `IJSObjectReference`,
   `DotNetObjectReference`, and observer subscription. JS-side resources must be
   `disconnect`-ed AND the `IJSObjectReference` itself disposed.
4. **Catch teardown exceptions** — `JSDisconnectedException` (Server circuit
   tear-down) and `ObjectDisposedException` (module already disposed) are
   expected and should be swallowed during `DisposeAsync`.
5. **Prerender-safe** — interop calls only inside `OnAfterRenderAsync`; check
   `firstRender` for one-time setup. Never call `IJSRuntime` in `OnInitialized*`.

## Features (Two-Way Bindable)

```csharp
[Parameter] public IReadOnlyList<TItem>? Data { get; set; }

[Parameter] public DataGridSelectionMode SelectionMode { get; set; }
[Parameter] public IReadOnlyList<TItem>? SelectedItems { get; set; }
[Parameter] public EventCallback<IReadOnlyList<TItem>> SelectedItemsChanged { get; set; }

[Parameter] public SortDescriptor? Sort { get; set; }
[Parameter] public EventCallback<SortDescriptor?> SortChanged { get; set; }

[Parameter] public string? QuickFilter { get; set; }
[Parameter] public EventCallback<string?> QuickFilterChanged { get; set; }
```

Side-effect callbacks use the `On*` prefix:

```csharp
[Parameter] public EventCallback<TItem> OnRowEdit { get; set; }
[Parameter] public EventCallback<List<BatchEditChange<TItem>>> OnBatchCommit { get; set; }
[Parameter] public EventCallback<DataGridState> OnStateChanged { get; set; }
[Parameter] public EventCallback<TItem> OnContextMenu { get; set; }
```

## Capability Surface

- Sorting (single + multi-column with priority badges, Shift+Click)
- Filtering (per-column operators) + quick text filter
- Grouping (`GroupBy` by property name, or `GroupByField` lambda)
- Selection (`None`, `Single`, `Multiple` with select-all checkbox column)
- Inline editing + batch edit mode with commit/discard
- Virtualization (`VirtualizeRows`, `ItemSize`, `OverscanCount`)
- Infinite scroll (sentinel + `IntersectionObserver` from JS module)
- Pagination
- Server-side data via `LoadData` + `ServerTotalCount`
- Export (CSV, Excel, PDF — see methods on grid)
- State persistence (`StateKey` → localStorage)
- Detail row template, context-menu template, empty template
- Cell validation, command column, conditional formatting, cell merge, row reorder

## Sample Skeleton

```razor
<ArcadiaDataGrid TItem="Employee"
                 Data="@employees"
                 SelectionMode="DataGridSelectionMode.Multiple"
                 @bind-SelectedItems="selected"
                 @bind-Sort="sort"
                 PageSize="25">
    <ArcadiaColumn Property="Name" Sortable Filterable />
    <ArcadiaColumn Property="Department" Sortable />
    <ArcadiaColumn Property="Salary" Format="C0" Aggregate="AggregateType.Sum" />
</ArcadiaDataGrid>
```
