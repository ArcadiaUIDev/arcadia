# PRP: DataGrid Enterprise Features

## Goal
Add features that enterprise evaluators expect: batch editing, state persistence, context menu, and Excel export.

## Scope

### 1. Batch Editing
- `BatchEdit="true"` parameter
- Modified cells tracked in a change set (highlighted with CSS)
- Toolbar: "Save Changes" and "Discard" buttons
- `OnBatchCommit` callback with `List<(TItem Item, string Column, object OldValue, object NewValue)>`
- Undo per cell (Ctrl+Z in edit mode)

### 2. Persistent State
- `StateKey="my-grid"` parameter — localStorage key
- Saves: column order, column widths, column visibility, sort state, filter state, page size
- `OnStateChanged` callback for server-side persistence
- `RestoreState(string json)` / `SaveState()` methods for manual control
- JS interop for localStorage read/write

### 3. Context Menu
- `ContextMenuTemplate` RenderFragment<TItem> parameter
- Right-click row → show positioned menu
- Built-in actions: Copy Row, Copy Cell, Export Selected
- `OnContextMenu` callback
- Keyboard: Shift+F10 or context menu key

### 4. Excel (XLSX) Export
- `ExportToExcelAsync()` method
- Uses Open XML SDK (lightweight, no COM)
- Respects column formats, visibility, sort/filter
- Optional: include headers, auto-width columns, sheet name
- Package dependency: `DocumentFormat.OpenXml` (conditional, only when used)

### 5. Cell Merging / Spanning
- `ColSpan` parameter on ArcadiaColumn (dynamic, per-row)
- `RowSpan` callback `Func<TItem, int>` for vertical merge
- CSS Grid handles the layout

### 6. Server-Side Export
- When `LoadData` is set, export fetches ALL data (not just current page)
- `ExportLoadData` callback for custom export queries
- Progress indicator during large exports

## Files to Modify
- `ArcadiaDataGrid.razor.cs` — batch edit tracking, state save/restore, context menu
- `ArcadiaDataGrid.razor` — batch edit UI, context menu popup, progress bar
- `datagrid-interop.js` — localStorage, context menu positioning, file download
- `arcadia-datagrid.css` — modified cell highlight, context menu, progress

## Files to Create
- `Core/DataGridState.cs` — serializable state model
- `Core/BatchEditChange.cs` — change tracking model
- `Services/ExcelExportService.cs` — XLSX generation
- `Components/DataGridContextMenu.razor` — context menu component

## Dependencies
- `DocumentFormat.OpenXml` — for Excel export (optional, loaded on demand)

## Tests
- Batch edit: modify 3 cells, commit, verify callback
- Batch edit: modify 2 cells, discard, verify original values
- State persistence: sort + filter, reload, verify restored
- Context menu: right-click, verify menu appears
- Excel export: verify file downloads with correct data
- Cell merge: ColSpan=2, verify cell spans 2 columns

## Acceptance
- [ ] Batch editing with change tracking and save/discard
- [ ] State persistence to localStorage
- [ ] Context menu with built-in actions
- [ ] Excel export via Open XML
- [ ] Cell merging/spanning
- [ ] Server-side export with progress
- [ ] All existing tests pass
