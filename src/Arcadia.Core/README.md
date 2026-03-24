<p align="center">
  <strong>Arcadia.Core</strong><br>
  <em>The foundation of Arcadia Controls — base classes, theming, and accessibility for Blazor</em>
</p>

## You probably don't need to install this directly

Arcadia.Core is automatically pulled in when you install `Arcadia.Charts`, `Arcadia.FormBuilder`, or any other Arcadia package. Install it directly only if you're building custom components that extend the Arcadia base classes.

```bash
dotnet add package Arcadia.Core
```

## What's Inside

| Module | What it does |
|--------|-------------|
| `ArcadiaComponentBase` | Base class with `Class`, `Style`, `AdditionalAttributes` — every Arcadia component inherits this |
| `ArcadiaInputBase<T>` | Two-way binding base (`Value`/`ValueChanged`/`ValueExpression`) for form inputs |
| `CssBuilder` / `StyleBuilder` | Fluent API for building CSS classes and inline styles without string concatenation |
| `FocusTrap` | Traps keyboard focus inside modals/dialogs for accessibility |
| `LiveRegion` | ARIA live region for screen reader announcements |
| `IdGenerator` | Unique IDs for `aria-describedby` and `label[for]` linking |

## Why it matters

- **Zero external dependencies** — only `Microsoft.AspNetCore.Components`
- **WCAG 2.1 AA** — accessibility baked into the base classes, not bolted on
- **Multi-target** — .NET 5, 6, 7, 8, 9, 10
- **All render modes** — Server, WASM, Auto, SSR

**[Documentation](https://arcadiaui.com/docs)** · **[GitHub](https://github.com/ArcadiaUIDev/arcadia)**
