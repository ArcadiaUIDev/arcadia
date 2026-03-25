# Changelog

All notable changes to Arcadia Controls are documented here. This project uses [Keep a Changelog](https://keepachangelog.com/) format and [Semantic Versioning](https://semver.org/).

## [1.0.0-beta.8] — 2026-03-24

### Added
- **Sankey Diagram** (`ArcadiaSankeyChart`) — flow visualization with topological column layout, 4+ level support, cycle handling, input validation (negative values, self-links, duplicates)
- **Chord Diagram** (`ArcadiaChordChart`) — circular relationship visualization with arc ring, quadratic bezier ribbon paths, rotated labels, hover highlight, small-arc label suppression
- **30+ configurable visual parameters** across all 16 charts:
  - ChartBase: `GridOpacity`, `AxisLineOpacity`, `PointRadius`
  - Pie/Rose: `SliceStrokeWidth`, `SliceStrokeColor`
  - Treemap: `CellStrokeWidth`, `CellStrokeColor`, `CellLabelColor`
  - Funnel: `StageOpacity`, `StageLabelColor`
  - Scatter: `PointOpacity`, `TrendlineOpacity`
  - Candlestick: `WickWidth`, `CandleWidthRatio`
  - BoxPlot: `BoxFillOpacity`, `WhiskerOpacity`, `MedianLineColor`, `MedianLineWidth`
  - Waterfall: `ConnectorOpacity`, `ConnectorDashPattern`
  - RangeArea: `LineStrokeWidth`
  - Gauge: `TrackOpacity`
  - Heatmap: `CellGap`
  - Radar: `GridRingOpacity`, `HoverDimOpacity`, `HoverFillOpacity`
  - Sankey: `LinkOpacity`, `LinkHoverOpacity`
  - Chord: `ChordOpacity`, `ChordHoverOpacity`, `MinLabelAngle`
- Radar chart hover-dim effect (hovering one series dims all others)
- Chord chart hover highlight (ribbons at 0.75 default, 1.0 on hover)
- Blog post screenshots for dashboard tutorial
- 63 new unit tests (35 Sankey + 28 Chord), 18 new E2E tests with visual baselines

### Fixed
- **Animation opacity override** — Sankey, Chord, Range Area, and Radar fill animations no longer snap to `opacity: 1` after completion. Uses `from`-only keyframes respecting inline opacity.
- Gallery defaults to dark theme with gradient title shading

### Changed
- Community edition locked to 4 charts (Line, Bar, Pie, Scatter). Funnel, Treemap, and Waterfall moved to Pro tier.
- Total chart count: 14 → 16

## [1.0.0-beta.7] — 2026-03-24

### Added
- Custom tooltip templates (`TooltipTemplate` parameter) — render arbitrary Blazor content on hover
- Roslyn analyzers package (`Arcadia.Analyzers`)
- NuGet package READMEs rewritten

### Fixed
- Responsive charts finalized: `width="100%"` + viewBox scaling → ResizeObserver approach
- WASM playground synced with Server demo
- Deploy workflow fixes

## [1.0.0-beta.6] — 2026-03-23

### Added
- **Responsive width by default** — `Width="0"` fills container correctly

### Fixed
- BlogLayout not reading MDX frontmatter props

## [1.0.0-beta.5] — 2026-03-23

### Fixed
- `Loading` parameter shows skeleton shimmer correctly
- Annotations render at correct data index positions
- ResizeObserver listener leak on dispose
- All known API issues from external review addressed

### Breaking Changes
- `OnPointClick` changed from `EventCallback<T>` to `EventCallback<PointClickEventArgs<T>>` — now includes `Item`, `DataIndex`, `SeriesIndex`

## [1.0.0-beta.4] — 2026-03-22

### Added
- **Box Plot** (`ArcadiaBoxPlot<T>`) — statistical distribution chart
- **Range Area** (`ArcadiaRangeAreaChart<T>`) — confidence intervals and min/max bands
- Click events (`OnPointClick`) on Bar, Pie, Line, Scatter

### Fixed
- Empty state: all charts show `NoDataState` when data is null/empty
- NaN in screen reader tables replaced with "—"
- CSS variable fallbacks on all color references

### Breaking Changes
- `helix.css` renamed to `arcadia.css` — update `<link>` tags

## [1.0.0-beta.3] — 2026-03-22

### Added
- **Helix → Arcadia rename** across all components, namespaces, CSS, packages
- License key validation (`ArcadiaLicense.SetKey()`)
- Community watermark on Pro components
- WASM playground at `/playground/`
- MCP server for AI-assisted code generation
- IDE snippets (Visual Studio + JetBrains Rider)
- Rose/Polar chart, stacked area, combo charts
- Annotations, crosshair, export toolbar
- Interactive legend toggle
- Streaming data with `AppendAndSlide`
- Playwright E2E tests

### Breaking Changes
- **All names changed**: `HelixKpiCard` → `ArcadiaKpiCard`, `HelixLineChart` → `ArcadiaLineChart`, etc.
- **All CSS renamed**: `helix-theme.css` → `arcadia-theme.css`, `helix-charts.css` → `arcadia-charts.css`
- **All namespaces**: `HelixUI.*` → `Arcadia.*`
- **NuGet packages**: `HelixUI.Charts` → `Arcadia.Charts`, etc.

## [1.0.0-beta.2] — 2026-03-21

### Added
- Candlestick (OHLC) chart
- On-load animations for all chart types
- 30+ parameters on `ChartBase<T>` (axes, grid, margins, pan/zoom, crosshair)
- Pro charts: Radar, Gauge, Heatmap, Funnel, Treemap, Waterfall
- Trendlines (linear regression, moving average)
- Stacked bar mode
- Anti-collision layout engine
- 38 bUnit unit tests

## [1.0.0-beta.1] — 2026-03-20

### Added
- Initial release
- **Core**: base classes, CSS builder, focus trap, accessibility utilities
- **Theme**: design tokens, CSS custom properties, light/dark themes, Tailwind plugin
- **Charts**: Line, Bar, Pie, Scatter + 7 dashboard widgets
- **Form Builder**: 21 field types, schema-driven, wizard mode, validation
- **Notifications**: toast system with auto-dismiss and stacking
- Multi-target: .NET 5–10
- Server, WebAssembly, and Auto render mode support
