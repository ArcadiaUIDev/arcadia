# CLAUDE.md — HelixUI Root (Architect Context)

## Project Overview
**HelixUI** is a commercial Blazor component library targeting enterprise .NET developers.
Multi-targets .NET 5 through .NET 10. Supports Blazor Server, WebAssembly, and Auto (net8+) render modes.

## Repository Structure
```
/helixui
├── src/
│   ├── Arcadia.Core/           # Shared utilities, base classes, theming engine
│   ├── Arcadia.Theme/          # Design tokens, CSS, Tailwind plugin
│   ├── Arcadia.Charts/         # 20 chart types + 7 dashboard widgets (pure SVG)
│   ├── Arcadia.DataGrid/       # High-performance Blazor DataGrid (pure C#, no AG Grid)
│   ├── Arcadia.FormBuilder/    # Dynamic form builder (21 field types)
│   ├── Arcadia.DashboardKit/   # Drag-and-drop dashboard grid with FLIP animations
│   ├── Arcadia.UI/             # 46 general-purpose UI components (Dialog, Tabs, Card, etc.)
│   ├── Arcadia.Gauge/          # Free standalone gauge component (MIT)
│   ├── Arcadia.Notifications/  # Toast notification system
│   ├── Arcadia.Analyzers/      # Roslyn analyzers for Arcadia API usage
│   ├── Arcadia.RichText/       # (placeholder — not yet implemented)
│   ├── Arcadia.FileManager/    # (placeholder — not yet implemented)
│   ├── Arcadia.Scheduler/      # (placeholder — not yet implemented)
│   └── Arcadia.Workflow/       # (placeholder — not yet implemented)
├── tests/
│   ├── HelixUI.Tests.Unit/     # bUnit tests (1,161+)
│   └── HelixUI.Tests.E2E/      # Playwright tests (251+)
├── samples/
│   ├── HelixUI.Demo.Server/    # Server-side demo app
│   └── HelixUI.Demo.Wasm/      # WASM demo app
├── website/                     # Astro documentation site (arcadiaui.com)
└── tools/                       # MCP server, IDE snippets
```

## Architecture Principles
1. **Render mode agnostic** — every component MUST work in Server, WASM, and Auto modes
2. **Zero Bootstrap dependency** — Tailwind CSS compatible theming via CSS custom properties
3. **Accessibility first** — WCAG 2.1 AA minimum for all components
4. **Minimal JS interop** — use native Blazor when possible, JS only when necessary
5. **Consistent API surface** — all components follow the same parameter naming conventions
6. **Tree-shakeable** — each component is a separate NuGet package, Core is the only shared dep
7. **Performance budgeted** — render time, memory, and interop call counts are tracked

## Coding Standards
- C# 13+ features, nullable reference types enabled, strict mode
- `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>`
- Central package management via `Directory.Packages.props`
- Shared build config via `Directory.Build.props`
- All public APIs must have XML documentation
- No `async void` — ever
- IAsyncDisposable for any component with JS interop
- Parameter naming: `Value`, `ValueChanged`, `ValueExpression` for two-way binding
- Event-callback naming rule:
  - **`<Property>Changed`** — paired with `<Property>` so `@bind-<Property>` works
    (e.g. `Value` + `ValueChanged`, `Sort` + `SortChanged`, `SelectedItems` + `SelectedItemsChanged`,
    `QuickFilter` + `QuickFilterChanged`). Required by the Blazor framework — do not invent a
    different convention for two-way binding.
  - **`On<Verb>`** — side-effect callbacks that do NOT write back into a parameter
    (e.g. `OnClick`, `OnSubmit`, `OnPointClick`, `OnRowEdit`, `OnBatchCommit`, `OnContextMenu`).
  - When in doubt: ask "does this fire to notify the parent that a bindable parameter
    changed?" — yes → `*Changed`; no → `On*`.
- CSS class parameter: always named `Class` (additional CSS), never override root class

## NuGet Package Naming
- `Arcadia.Core`
- `Arcadia.Theme`
- `Arcadia.Charts`
- `Arcadia.DataGrid`
- `Arcadia.FormBuilder`
- `Arcadia.DashboardKit`
- `Arcadia.UI`
- `Arcadia.Gauge`
- `Arcadia.Notifications`
- `Arcadia.Analyzers`

## Target Frameworks
- **Multi-target: net5.0, net6.0, net7.0, net8.0, net9.0, net10.0**
- Use `#if` preprocessor directives for API differences between versions
- Common TFM conditionals: `NET5_0_OR_GREATER`, `NET6_0_OR_GREATER`, `NET7_0_OR_GREATER`, `NET8_0_OR_GREATER`, `NET9_0_OR_GREATER`, `NET10_0_OR_GREATER`
- Blazor render modes (Server/WASM/Auto) only exist in net8.0+; for net5-7, only Server and WASM (separate hosting models)
- Each csproj must use conditional PackageReference for Microsoft.AspNetCore.Components matching the TFM
- Test against ALL target frameworks before committing

## Key Dependencies
- Microsoft.AspNetCore.Components (Blazor) — version matched per TFM
- No third-party C# dependencies in Core (keep it clean)
- Tailwind CSS 4.x (for Theme package)
- DataGrid is pure C# (no AG Grid dependency — uses Blazor Virtualize)

## Documentation Rules
- **Every new or changed public API must have documentation updated in the same commit**
- New `[Parameter]` → update the component's doc page in `website/src/pages/docs/`
- New component → create doc page AND add to `DocsLayout.astro` sidebar nav AND update the overview page
- Changed behavior → update the relevant doc section
- Doc pages live in `website/src/pages/docs/` as `.mdx` files
- Charts overview (`website/src/pages/docs/charts/index.mdx`) must list ALL chart types

## Git Conventions
- Branch naming: `feature/<component>/<description>`, `fix/<component>/<description>`
- Conventional commits: `feat(datagrid): add column sorting`
- PR template with checklist: render modes tested, accessibility checked, docs updated

## When You Are the Architect
You are the technical lead for HelixUI. Your responsibilities:
- Enforce consistency across all component packages
- Design base classes and shared abstractions in Core
- Review all cross-component concerns (theming, accessibility, disposal)
- Make build/packaging decisions
- Resolve architectural disputes between specialist agents
- Own the CI/CD pipeline and release process
