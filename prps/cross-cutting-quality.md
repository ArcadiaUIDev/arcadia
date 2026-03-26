# PRP: Cross-Cutting Quality & Trust

## Goal
Address the trust-building gaps flagged by the reviewer: tests, benchmarks, i18n, and audit artifacts.

## Scope

### 1. DataGrid Unit Test Suite
- Target: 100+ tests covering every feature
- Structure:
  - `DataGridRenderTests.cs` — basic render, empty state, loading skeleton
  - `DataGridSortTests.cs` — single sort, multi-sort (when implemented), direction toggle
  - `DataGridFilterTests.cs` — all 6 operators, empty filter, clear filter
  - `DataGridPagingTests.cs` — page navigation, page size change, total count
  - `DataGridSelectionTests.cs` — single select, multi select, select all, deselect
  - `DataGridGroupingTests.cs` — group render, expand/collapse, sorting within groups
  - `DataGridEditTests.cs` — start edit, commit, cancel, double-commit guard
  - `DataGridAccessibilityTests.cs` — ARIA roles, keyboard navigation, live regions
  - `DataGridVirtualScrollTests.cs` — Virtualize renders, item size, overscan
  - `DataGridExportTests.cs` — CSV content, respects filters/visibility
  - `DataGridColumnTests.cs` — Key resolution, format, templates, visibility
  - `DataGridThemeTests.cs` — 6 themes apply correct CSS classes

### 2. FormBuilder Field Type Tests
- One test file per field type (30+ types)
- Each covers: rendering, value binding, validation, disabled state
- Auto-generate test templates for remaining types

### 3. Published Performance Benchmarks
- Create `/benchmarks/` directory with BenchmarkDotNet project
- Benchmarks:
  - DataGrid: 100/1K/10K/100K rows render time
  - DataGrid: sort 10K rows, filter 10K rows
  - LineChart: 100/1K/10K points render time
  - PieChart: 5/20/50 slices render time
  - Theme switch: measure re-render time
- Publish results in `/docs/benchmarks.md`
- CI: run benchmarks on release, fail if regression > 20%

### 4. Localization / RTL Support
- `IStringLocalizer` integration for all UI strings
- Built-in strings: "No data available", "Page X of Y", "Filter", "Export CSV", etc.
- Resource files for: en, de, fr, es, ja, zh
- RTL: `dir="rtl"` on grid container, mirror column order
- Charts: RTL axis labels, legend position

### 5. Accessibility Audit Artifacts
- Run axe-core on every component page programmatically
- Generate HTML report with pass/fail per component
- Publish at `/docs/accessibility-audit.html`
- CI: fail on new violations

### 6. Test Coverage Metrics
- Add Coverlet to unit test project
- Generate lcov/cobertura report
- Publish coverage badge in README
- Minimum thresholds: Core 90%, Charts 80%, DataGrid 80%, Forms 70%

## Files to Create
- `tests/Arcadia.Tests.Unit/DataGrid/` — 12 test files
- `tests/Arcadia.Tests.Unit/FormBuilder/` — 30+ test files
- `benchmarks/Arcadia.Benchmarks/` — BenchmarkDotNet project
- `src/Arcadia.Core/Localization/` — resource files + IStringLocalizer adapter

## Files to Modify
- `tests/Arcadia.Tests.Unit/Arcadia.Tests.Unit.csproj` — add Coverlet
- `.github/workflows/ci.yml` — coverage report, benchmark job
- `README.md` — coverage badge
- All component `.razor` files — replace hardcoded strings with localizer calls

## Acceptance
- [ ] 100+ DataGrid unit tests passing
- [ ] Every FormBuilder field type has at least 3 tests
- [ ] BenchmarkDotNet results published
- [ ] Localization works for at least 3 languages
- [ ] RTL layout renders correctly
- [ ] axe-core audit report generated in CI
- [ ] Coverage badge in README shows > 80%
