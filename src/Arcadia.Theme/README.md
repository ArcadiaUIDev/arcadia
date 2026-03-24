<p align="center">
  <strong>Arcadia.Theme</strong><br>
  <em>Design tokens, dark/light themes, and CSS custom properties for Blazor</em>
</p>

## Quick Start

```bash
dotnet add package Arcadia.Theme
```

Add one line to your `App.razor`:
```html
<link href="_content/Arcadia.Theme/css/arcadia.css" rel="stylesheet" />
```

That's it. You get 80+ CSS custom properties, dark mode, and density variants.

## What You Get

**Colors** — `--arcadia-color-primary`, `--arcadia-color-success`, `--arcadia-color-danger`, and 20+ semantic color tokens that adapt to light/dark mode automatically.

**Spacing** — Consistent spacing scale from `--arcadia-spacing-1` (4px) to `--arcadia-spacing-16` (64px).

**Typography** — Font sizes, weights, and line heights via `--arcadia-text-sm`, `--arcadia-font-bold`, etc.

**Dark Mode** — Wrap your app in `<ThemeProvider>` and call `ThemeService.ToggleTheme()`. All Arcadia components adapt instantly.

```csharp
// Program.cs
builder.Services.AddScoped<ThemeService>();
```

```razor
<ThemeProvider>
    <Router ... />
</ThemeProvider>
```

## Tailwind CSS Compatible

All tokens map to Tailwind utility classes via the Arcadia Tailwind plugin. Use `bg-arcadia-primary` alongside standard Tailwind classes.

**[Docs](https://arcadiaui.com/docs/theme)** · **[GitHub](https://github.com/ArcadiaUIDev/arcadia)**
