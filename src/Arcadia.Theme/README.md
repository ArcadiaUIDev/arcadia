# Arcadia.Theme

Design tokens, theming engine, and Tailwind CSS integration for Blazor.

## Quick Start

```html
<link href="_content/Arcadia.Theme/css/helix.css" rel="stylesheet" />
```

```csharp
builder.Services.AddScoped<ThemeService>();
```

```razor
<ThemeProvider>@Body</ThemeProvider>
```

## Features

- CSS custom property design tokens (colors, spacing, typography, elevation)
- Light and dark themes with WCAG 2.1 AA contrast ratios
- Three density modes (compact, default, comfortable)
- Tailwind CSS 4.x plugin
- Custom theme creation

## Installation

```
dotnet add package Arcadia.Theme
```
