# DataGrid Virtual Scrolling + Smooth Scroll — PRP v2

## Problem

1. **Performance cliff**: Grid renders ALL rows on the current page as DOM elements. At PageSize=100+ the browser struggles. At PageSize=0 (show all) with 10K rows, the page is unusable.
2. **Scroll feel**: Standard browser scroll is jank-free but the grid currently uses no scroll optimization. When Height is set and content overflows, the browser handles scroll but re-renders the entire tbody on every state change.
3. **No infinite scroll option**: Users must click pagination buttons. Modern grids (AG Grid, Notion) use smooth virtual scroll where the user just scrolls and data loads seamlessly.

## What We're Building

A dual-mode scroll system:
- **Mode A: Client-side virtual scroll** — All data in memory, Blazor `Virtualize<T>` renders only visible rows
- **Mode B: Server-side virtual scroll** — Data loaded on demand via `ItemsProvider`, server returns pages as user scrolls

Plus CSS smooth scroll polish on the existing paginated mode.

---

## Technical Design

### The Blazor Virtualize<T> Component

`Microsoft.AspNetCore.Components.Web.Virtualization.Virtualize<TItem>` is built into Blazor (net6.0+). It:

- Renders only visible items + overscan buffer
- Manages a spacer `<div>` above and below to maintain correct scroll position
- Supports two modes:
  - `Items` — pass a full `ICollection<T>`, component handles windowing
  - `ItemsProvider` — pass a delegate, component calls it with `(startIndex, count)` as user scrolls
- Key parameters: `ItemSize` (row height estimate), `OverscanCount` (buffer rows)

### Problem: Virtualize renders INSIDE a container, not inside `<tbody>`

Blazor's `Virtualize<T>` renders a flat list of elements. It CANNOT wrap its output in `<tbody>` because it injects spacer `<div>`s for scroll calculation. Putting it inside a `<table>` produces invalid HTML:

```html
<table>
  <thead>...</thead>
  <!-- Virtualize renders: -->
  <div style="height: 4000px"></div>  <!-- spacer — INVALID inside table -->
  <tr>...</tr>
  <tr>...</tr>
  <div style="height: 6000px"></div>  <!-- spacer — INVALID inside table -->
</table>
```

### Solution: CSS Grid layout for virtual mode

When `VirtualizeRows=true`, switch from `<table>` to a CSS Grid-based layout:

```html
<div class="arcadia-grid__virtual-container" style="height: {Height}; overflow-y: auto;">
  <!-- Header rendered as a sticky div -->
  <div class="arcadia-grid__virtual-header">
    <div class="arcadia-grid__virtual-cell">Name</div>
    <div class="arcadia-grid__virtual-cell">Dept</div>
    ...
  </div>

  <!-- Virtualize renders rows as divs -->
  <Virtualize Items="@allData" ItemSize="@ItemSize" OverscanCount="@OverscanCount" Context="item">
    <div class="arcadia-grid__virtual-row">
      <div class="arcadia-grid__virtual-cell">@item.Name</div>
      <div class="arcadia-grid__virtual-cell">@item.Dept</div>
      ...
    </div>
  </Virtualize>
</div>
```

This avoids the `<table>` + `<div>` spacer conflict entirely.

### Alternative: Keep `<table>` with a tbody wrapper

Some libraries work around this by wrapping `Virtualize` output in a fixed-height `<tbody>` with `overflow: auto` and using `position: absolute` rows. This is fragile.

**Decision: Use the CSS Grid approach.** It's cleaner, more flexible, and what AG Grid / Notion actually do under the hood.

---

## Implementation Plan

### Step 1: Add virtual row rendering to the razor template

In `ArcadiaDataGrid.razor`, after the existing `<table>` block:

```razor
@if (VirtualizeRows && _columnsCollected)
{
    <div class="arcadia-grid__scroll-container" style="height: @Height; overflow-y: auto; scroll-behavior: smooth;">
        <!-- Sticky header -->
        <div class="arcadia-grid__virtual-header" style="@GetGridTemplateColumns()">
            @foreach (var col in Columns.Where(c => c.IsVisible))
            {
                <div class="arcadia-grid__virtual-hcell">@col.Title</div>
            }
        </div>

        <!-- Virtualized rows -->
#if NET6_0_OR_GREATER
        <Microsoft.AspNetCore.Components.Web.Virtualization.Virtualize
            TItem="TItem"
            Items="@GetAllSortedDataList()"
            ItemSize="@ItemSize"
            OverscanCount="@OverscanCount"
            Context="item">
            <div class="arcadia-grid__virtual-row" style="@GetGridTemplateColumns()">
                @foreach (var col in Columns.Where(c => c.IsVisible))
                {
                    <div class="arcadia-grid__virtual-cell">
                        @if (col.Template is not null) { @col.Template(item) }
                        else if (col.Field is not null) { @col.FormatValue(col.Field(item)) }
                    </div>
                }
            </div>
        </Microsoft.AspNetCore.Components.Web.Virtualization.Virtualize>
#else
        @foreach (var item in GetAllSortedDataList())
        {
            <div class="arcadia-grid__virtual-row" style="@GetGridTemplateColumns()">
                @foreach (var col in Columns.Where(c => c.IsVisible))
                {
                    <div class="arcadia-grid__virtual-cell">
                        @if (col.Template is not null) { @col.Template(item) }
                        else if (col.Field is not null) { @col.FormatValue(col.Field(item)) }
                    </div>
                }
            </div>
        }
#endif
    </div>
}
else if (_columnsCollected && Columns.Count > 0)
{
    <!-- Existing <table> rendering -->
}
```

### Step 2: CSS Grid styles

```css
/* Virtual scroll container */
.arcadia-grid__scroll-container {
  overflow-y: auto;
  scroll-behavior: smooth;
  -webkit-overflow-scrolling: touch;
}

/* Virtual header — sticky at top */
.arcadia-grid__virtual-header {
  display: grid;
  position: sticky;
  top: 0;
  z-index: 2;
  background: var(--_header);
  border-bottom: 1px solid var(--_border);
}

/* Virtual row */
.arcadia-grid__virtual-row {
  display: grid;
  border-bottom: 1px solid var(--_border);
  transition: background 0.1s;
}
.arcadia-grid__virtual-row:hover {
  background: var(--_row-hover);
}

/* Virtual cell */
.arcadia-grid__virtual-cell,
.arcadia-grid__virtual-hcell {
  padding: 11px 16px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}
.arcadia-grid__virtual-hcell {
  font-weight: 600;
  font-size: 0.7rem;
  text-transform: uppercase;
  letter-spacing: 0.06em;
  color: var(--_muted);
}
```

### Step 3: Grid template columns helper

```csharp
private string GetGridTemplateColumns()
{
    var cols = Columns.Where(c => c.IsVisible).ToList();
    var template = string.Join(" ", cols.Select(c => c.Width ?? "1fr"));
    return $"grid-template-columns: {template};";
}
```

This generates something like `grid-template-columns: 60px 1fr 1fr 1fr 120px 100px 100px;`

### Step 4: Server-side ItemsProvider (net6+)

```csharp
#if NET6_0_OR_GREATER
private async ValueTask<ItemsProviderResult<TItem>> ProvideItems(
    ItemsProviderRequest request)
{
    if (!_isServerMode)
    {
        var allData = GetAllSortedDataList();
        var page = allData.Skip(request.StartIndex).Take(request.Count).ToList();
        return new ItemsProviderResult<TItem>(page, allData.Count);
    }

    // Server mode: call LoadData with the virtualized range
    var args = new DataGridLoadArgs
    {
        Skip = request.StartIndex,
        Take = request.Count,
        SortProperty = _currentSort?.Property,
        SortDirection = _currentSort?.Direction ?? SortDirection.None,
        Filters = _filters.Values.Where(f => !string.IsNullOrEmpty(f.Value)).ToList()
    };
    await LoadData.InvokeAsync(args);
    return new ItemsProviderResult<TItem>(
        Data?.ToList() ?? new List<TItem>(),
        ServerTotalCount ?? 0);
}
#endif
```

### Step 5: Smooth scroll CSS for non-virtual mode

Even without virtualization, the existing table scroll can be smoothed:

```css
.arcadia-grid {
  scroll-behavior: smooth;
  -webkit-overflow-scrolling: touch;
}

/* Smooth scrollbar styling */
.arcadia-grid::-webkit-scrollbar {
  width: 8px;
  height: 8px;
}
.arcadia-grid::-webkit-scrollbar-track {
  background: var(--_bg);
}
.arcadia-grid::-webkit-scrollbar-thumb {
  background: var(--_border);
  border-radius: 4px;
}
.arcadia-grid::-webkit-scrollbar-thumb:hover {
  background: var(--_muted);
}
```

---

## Conditional Compilation Strategy

The `Virtualize<T>` component doesn't exist on net5.0. Strategy:

```razor
@* In the .razor file, use #if in the @code block for the ItemsProvider *@
@* In the template, wrap Virtualize in a helper component or use RenderFragment *@
```

Actually, Razor files don't support `#if`. The approach:

1. Create a `VirtualRowRenderer.razor` component that wraps `Virtualize<T>` and only exists when net6+
2. Or: use a `RenderFragment` approach where the code-behind builds the fragment conditionally
3. **Simplest: just require net6+ for virtual scroll.** Document that `VirtualizeRows=true` requires net6.0 or later. On net5, ignore the parameter silently and render full rows.

**Decision: Option 3.** The `Virtualize` template code goes in a separate partial `.razor` file that uses `#if NET6_0_OR_GREATER` in the code-behind. The template itself always renders the virtual structure — on net5, it falls back to a foreach.

---

## Demo Page Update

Update `TestDataGrid.razor` to include a virtual scroll demo:

```razor
<h3>Virtual Scroll — 10,000 Rows</h3>
<ArcadiaDataGrid TItem="BigDataItem" Data="@_bigData"
                  VirtualizeRows="true" Height="400px"
                  Class="arcadia-grid--obsidian">
    <ArcadiaColumn TItem="BigDataItem" Field="@(e => (object)e.Id)" Title="ID" Width="80px" />
    <ArcadiaColumn TItem="BigDataItem" Field="@(e => (object)e.Name)" Title="Name" />
    <ArcadiaColumn TItem="BigDataItem" Field="@(e => (object)e.Value)" Title="Value" Format="N2" />
</ArcadiaDataGrid>
```

---

## Files to Create/Modify

| File | Action |
|------|--------|
| `ArcadiaDataGrid.razor` | Add virtual rendering path before existing table |
| `ArcadiaDataGrid.razor.cs` | Add GetGridTemplateColumns, GetAllSortedDataList, ProvideItems |
| `arcadia-datagrid.css` | Add virtual-* classes, smooth scroll, scrollbar styling |
| `TestDataGrid.razor` | Add 10K row virtual scroll demo section |

## Testing

- E2E: Render 10K rows with VirtualizeRows=true, verify < 30 DOM rows exist
- E2E: Scroll to bottom, verify last rows render
- E2E: Verify header stays sticky during scroll
- Performance: Measure initial render time for 10K rows
- Manual: Verify smooth scroll feel on Chrome, Firefox, Safari

## Success Criteria

- 10,000 rows render in < 100ms
- Scrolling at 60fps with no jank
- Only ~20-30 DOM rows exist at any time
- Header sticks during scroll
- Themes apply correctly to virtual rows
- Sort/filter work with virtual scroll
