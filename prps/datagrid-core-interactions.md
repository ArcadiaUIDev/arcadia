# PRP: DataGrid Core Interactions

## Goal
Add the table-stakes interactive features that every evaluator reaches for first.

## Scope

### 1. Multi-Column Sort
- Hold Shift+Click to add secondary/tertiary sort columns
- `SortDescriptor` becomes `List<SortDescriptor>` internally
- Show sort priority numbers (1, 2, 3) on column headers
- `SortChanged` callback receives the full sort list
- Keyboard: Shift+Enter on focused header adds to sort stack

### 2. Column Resizing (drag)
- JS interop already exists (`initResizeHandles`) — verify it works end-to-end
- Drag handle on right edge of each `<th>`
- Persist width in column state
- Cursor: `col-resize` on hover
- Min width: 50px
- `OnColumnResized` callback with column key + new width

### 3. Column Reordering (drag-and-drop)
- Drag state already exists (`_dragSourceCol`, `_dropTargetCol`, `StartColumnDrag`, `DropColumn`)
- Verify drag-and-drop works end-to-end in browser
- Visual indicator: ghost column during drag, drop target highlight
- `OnColumnReordered` callback
- Keyboard: Alt+Arrow to move focused column

### 4. Global Search / Quick Filter
- New parameter: `string? QuickFilter`
- Single input in toolbar that searches across all visible columns
- Case-insensitive contains match
- Debounce: 300ms
- Highlight matches in cells (optional, `HighlightSearch="true"`)

### 5. Copy to Clipboard
- Ctrl+C copies selected rows as TSV (tab-separated)
- If no selection, copies focused row
- Uses `navigator.clipboard.writeText()` via JS interop
- Header row included as first line

### 6. Row Reordering (drag-and-drop)
- `AllowRowReorder="true"` parameter
- Drag handle column (leftmost, 3 dots icon)
- `OnRowReordered` callback with old/new index
- Keyboard: Alt+Up/Down to move focused row

## Files to Modify
- `ArcadiaDataGrid.razor.cs` — sort stack, quick filter, clipboard, row reorder state
- `ArcadiaDataGrid.razor` — toolbar search input, drag handles, sort indicators
- `ArcadiaColumn.razor` — resize handle improvements
- `datagrid-interop.js` — clipboard write, row drag, resize polish
- `arcadia-datagrid.css` — drag ghost, search highlight, reorder handle

## Files to Create
- `Core/QuickFilterService.cs` — debounced cross-column search

## Tests
- Multi-sort: 3 columns, toggle direction, clear
- Column resize: drag changes width, min width enforced
- Column reorder: drag A→B, verify order changed
- Quick filter: type "eng" → only Engineering rows
- Clipboard: select 2 rows, Ctrl+C, verify TSV
- Row reorder: drag row 3 to position 1

## Acceptance
- [ ] Multi-column sort with visual indicators
- [ ] Column resize via drag handle
- [ ] Column reorder via drag-and-drop
- [ ] Global search in toolbar
- [ ] Ctrl+C copies selected rows
- [ ] Row reorder with callback
- [ ] All existing tests still pass
- [ ] Demo gallery updated
