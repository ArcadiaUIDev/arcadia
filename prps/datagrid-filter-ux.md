# PRP: DataGrid Filter UX Overhaul

## Goal
Replace the bare text-input filter row with a proper filter UI that handles different data types.

## Scope

### 1. Filter Operator Dropdown
- Each filter cell gets a dropdown: Contains, Equals, StartsWith, EndsWith, GreaterThan, LessThan, NotEquals, IsEmpty, IsNotEmpty
- Default operator: Contains for strings, Equals for numbers/dates/booleans
- Persist operator selection per column

### 2. Boolean Filter
- Auto-detect boolean columns (Field returns `bool`)
- Render a tri-state dropdown: All / True / False (not a text input)
- CSS: styled select matching grid theme

### 3. Date Range Filter
- Auto-detect DateTime columns
- Render a pair of date inputs (from / to) instead of text
- Support: exact date, date range, before, after
- Keyboard: type dates in locale format

### 4. Number Range Filter
- Auto-detect numeric columns (int, double, decimal)
- Render min/max inputs with GT/LT operators
- Support: exact, range, greater than, less than

### 5. Column Chooser / Picker UI
- `ShowColumnPicker="true"` parameter (toolbar button)
- Dropdown with checkboxes for each column
- Drag to reorder columns in the picker
- "Show All" / "Reset" buttons

### 6. Column Pinning UI
- Right-click column header → "Pin Left" / "Unpin"
- Or: pin icon in column header on hover
- Syncs with existing `Frozen` parameter

## Files to Modify
- `ArcadiaDataGrid.razor` — filter row with typed inputs, column picker dropdown, pin UI
- `ArcadiaDataGrid.razor.cs` — typed filter logic, column picker state
- `ArcadiaColumn.razor` — `DataType` parameter (auto-detected from Field return type)
- `Core/FilterDescriptor.cs` — add NotEquals, IsEmpty, IsNotEmpty operators
- `arcadia-datagrid.css` — filter dropdown, date picker, column picker panel

## Files to Create
- `Core/ColumnDataType.cs` — enum: String, Number, Date, Boolean, Custom
- `Components/DataGridColumnPicker.razor` — standalone picker panel

## Tests
- Boolean filter: toggle True/False/All
- Date range: set from/to, verify filtered data
- Number range: set min/max
- Operator dropdown: switch from Contains to Equals
- Column picker: hide 2 columns, verify they disappear
- Pin column: pin, verify sticky position

## Acceptance
- [ ] Operator dropdown per filter column
- [ ] Boolean columns use checkbox/dropdown filter
- [ ] Date columns use date range picker
- [ ] Number columns use min/max inputs
- [ ] Column picker with show/hide checkboxes
- [ ] Column pinning via UI (not just parameter)
- [ ] All existing tests pass
