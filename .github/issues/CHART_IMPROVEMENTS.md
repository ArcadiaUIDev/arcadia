# Chart Improvements — Master Tracker

## Pre-Flight Checklist (DO NOT FORGET)
For EVERY chart change, before considering it done:

- [ ] API is flexible (configurable colors, sizes, formats, positions)
- [ ] Tooltips work on hover (via ChartInteropService)
- [ ] On-load animation exists and respects prefers-reduced-motion
- [ ] Hover animations work (highlight, dim others)
- [ ] Legend toggle works (if multi-series)
- [ ] Hidden data table for screen readers
- [ ] aria-label on SVG container
- [ ] Works on dark AND light backgrounds
- [ ] Number formatting respects FormatProvider/CultureInfo
- [ ] Responsive — fills container width when Width=0
- [ ] SvgText used (never raw <text> in Razor)
- [ ] Tests written (rendering + edge cases)
- [ ] Documentation page updated on website
- [ ] Demo gallery tab shows the chart
- [ ] Blog code examples still compile
- [ ] Community repo synced if community feature

---

## Tier 1 — Must Fix

### 1.1 Wire Tooltips to ALL Charts
**Status:** 🔴 Not started
**Files:** All chart .razor files
**Scope:** Pie, Scatter, Candlestick, Radar, Heatmap, Waterfall, Funnel, Treemap
**Pattern:** Same as Line/Bar — @onmouseover calls ShowTooltipForPoint, @onmouseout calls HideTooltipAction
**Tooltip format:** Series/category name + formatted value (respect DataLabelFormatString)

### 1.2 Responsive Width
**Status:** 🔴 Not started
**Files:** ChartBase.cs, all chart .razor files, chart-interop.js
**Scope:** When Width=0, chart should fill its container and resize on window change
**Approach:** 
- Width=0 means "auto" — use ResizeObserver from JS interop
- On resize, update Width and re-render
- Need to debounce resize events (100ms)
- Charts currently hardcode 780px in gallery — change to Width="0"

### 1.3 Stacked Area Chart
**Status:** 🔴 Not started
**Files:** HelixLineChart.razor.cs
**Scope:** Add Stacked parameter to HelixLineChart — areas stack on top of each other
**API:** `<HelixLineChart Stacked="true" />`
**Behavior:** Each series area starts where the previous one ends
**Animation:** Same as regular area — fade in

### 1.4 Combo/Mixed Chart
**Status:** 🔴 Not started
**Files:** New HelixComboChart.razor + .cs
**Scope:** Line + Bar on same axes (e.g., revenue bars + trend line)
**API:**
```razor
<HelixComboChart TItem="Data" Data="@data" XField="@(d => d.Month)">
    <BarSeries Field="@(d => d.Revenue)" Name="Revenue" />
    <LineSeries Field="@(d => d.Trend)" Name="Trend" Dashed="true" />
</HelixComboChart>
```
**Or simpler:** Add SeriesType to SeriesConfig (Line, Bar, Area) and render mixed in existing chart

---

## Tier 2 — Makes It Competitive

### 2.1 Point Annotations
**Status:** 🔴 Not started
**Files:** New component or parameter on existing charts
**Scope:** Mark specific data points with labels/callouts
**API:**
```razor
<HelixLineChart Annotations="@annotations" />
```
Where annotations is a list of { X, Y, Label, Color }

### 2.2 Crosshair on Hover
**Status:** 🔴 Not started
**Files:** chart-interop.js, chart CSS
**Scope:** Vertical + horizontal lines following cursor within plot area
**API:** `<HelixLineChart ShowCrosshair="true" />`
**Needs:** JS mousemove tracking on SVG, render crosshair lines

### 2.3 Tooltip Number Formatting
**Status:** 🔴 Not started
**Files:** ChartBase.cs ShowTooltipForPoint method
**Scope:** Use DataLabelFormatString and FormatProvider in tooltip display
**Example:** Show "$142,580" not "142580"

### 2.4 Fix Gauge Animation
**Status:** 🔴 Not started
**Files:** arcadia-charts.css, HelixGaugeChart.razor
**Issue:** stroke-dashoffset animation doesn't work because path length varies per gauge value
**Fix:** Calculate actual path length and set stroke-dasharray/offset dynamically

---

## Tier 3 — Nice to Have

### 3.1 Box & Whisker Chart
**Status:** 🔴 Not started
**Use case:** Statistical distributions, outlier detection

### 3.2 Range Area Chart
**Status:** 🔴 Not started
**Use case:** Temperature ranges, confidence intervals, min/max bands

### 3.3 Synchronized Chart Groups
**Status:** 🔴 Not started
**Use case:** Hover on one chart highlights corresponding point on sibling charts

### 3.4 Polar/Rose Chart
**Status:** 🔴 Not started
**Use case:** Wind direction, cyclical data

---

## Post-Implementation Checklist
After ALL tier 1+2 items are done:
- [ ] Run full test suite (563+ tests must pass)
- [ ] Update homepage stats (chart count, test count)
- [ ] Update pricing page feature list
- [ ] Rebuild and sync community repo (community-relevant changes only)
- [ ] Update blog post code examples if API changed
- [ ] Update NuGet README files
- [ ] Tag new version and publish to NuGet
- [ ] Update llms.txt with new features
