# PRP: DataGrid DX Polish

## Goal
Fix the developer experience paper cuts flagged in the review.

## Scope

### 1. Fix Field Boxing DX
Current: `Field="@((Employee e) => (object)e.Salary)"` — requires explicit cast
Target: `Field="@(e => e.Salary)"` — just works

Options (pick one):
- **A) String-based Property**: Add `Property="Salary"` parameter using reflection. Simpler API but loses compile-time safety.
- **B) Expression-based Field**: Change `Func<TItem, object>` to `Expression<Func<TItem, object>>` — compiler auto-boxes value types in expression trees.
- **C) Keep Func but add implicit conversion**: Not possible in C#.

**Recommended: Option A** (add `Property` as alternative, keep `Field` for advanced cases).
```razor
<!-- Simple (90% of use cases) -->
<ArcadiaColumn Property="Salary" Title="Salary" Format="C0" />

<!-- Advanced (custom projections) -->
<ArcadiaColumn Field="@(e => $"{e.First} {e.Last}")" Title="Full Name" />
```

### 2. Document ChildContent/DetailTemplate Pattern
- Add explicit example in docs showing `<ChildContent>` wrapper when using `DetailTemplate`
- Add compiler-friendly error message when both are set without wrapper

### 3. Improve Empty State
- Current: just text "No data available"
- Add: `EmptyTemplate` RenderFragment for custom empty states
- Default: icon + message + optional action button
- CSS: centered, styled consistently with themes

### 4. Loading Skeleton Improvements
- Current: 5 fixed skeleton rows
- Match actual column widths in skeleton
- `SkeletonRowCount` parameter (default: PageSize or 5)
- Pulse animation respects `prefers-reduced-motion`

### 5. SelectionMode Enum
Current: separate `Selectable` + `MultiSelect` booleans
Target: single `SelectionMode` enum
```csharp
SelectionMode.None      // default
SelectionMode.Single    // click to select one
SelectionMode.Multiple  // checkbox column
```
Keep `Selectable`/`MultiSelect` as aliases for backwards compat.

## Files to Modify
- `ArcadiaColumn.razor` — add `Property` parameter, reflection resolver
- `ArcadiaDataGrid.razor` — empty template, skeleton improvements
- `ArcadiaDataGrid.razor.cs` — SelectionMode enum, Property resolution
- `arcadia-datagrid.css` — empty state styling, skeleton widths

## Files to Create
- `Core/DataGridSelectionMode.cs` — enum (already exists, verify)

## Tests
- Property="Salary" renders same as Field="@(e => (object)e.Salary)"
- EmptyTemplate renders custom content
- SelectionMode.Multiple adds checkbox column
- Skeleton matches column count

## Acceptance
- [ ] `Property` parameter works for simple column definitions
- [ ] `Field` still works for complex projections
- [ ] EmptyTemplate for custom empty states
- [ ] Improved skeleton matching columns
- [ ] SelectionMode enum (with backwards compat)
- [ ] Documentation updated
