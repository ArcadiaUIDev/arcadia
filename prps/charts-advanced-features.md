# PRP: Charts Advanced Features

## Goal
Close the feature gaps vs Syncfusion/Telerik flagged in the review.

## Scope

### 1. Synchronized Charts
- `SyncGroup="group1"` parameter on any chart
- Linked crosshair: hover on one chart shows crosshair on all charts in the same group
- Linked zoom/pan: zoom on one syncs all
- Implementation: shared state via CascadingValue or a SyncService
- Works across different chart types (line + bar synced)

### 2. Rubber Band Zoom (Selection Zoom)
- `ZoomMode="selection"` (in addition to existing "x", "y", "xy")
- Click-drag to draw a rectangle
- Release to zoom into selected region
- Double-click to reset zoom
- SVG overlay rect during selection

### 3. Financial Indicators (Candlestick)
- `Indicators` parameter: `List<IndicatorConfig>`
- Built-in: EMA, SMA, Bollinger Bands, MACD, RSI
- Each renders as an overlay series on the candlestick
- Separate Y-axis for RSI/MACD (oscillators)
- Color/style configurable per indicator

### 4. Drill-Down
- `OnDrillDown` callback with `DrillDownEventArgs` (item, series, level)
- Chart transitions to sub-category view with animation
- Breadcrumb trail for navigation back
- Works on Bar, Pie, Treemap charts

### 5. Multiple X-Axes
- `XAxisIndex` on SeriesConfig (mirrors existing `YAxisIndex`)
- Second X-axis at top of chart
- Independent scales and labels
- Use case: dual time scales, category + continuous

### 6. Print Support
- `PrintAsync()` method
- Renders chart at high-res with print-optimized CSS
- Removes interactive elements, uses solid colors
- Opens browser print dialog
- `@media print` CSS block

## Files to Modify
- `Core/ChartBase.cs` — SyncGroup, ZoomMode="selection"
- `Components/Charts/ArcadiaCandlestickChart.razor.cs` — indicators
- `Components/Charts/ArcadiaBarChart.razor.cs` — drill-down
- `Components/Charts/ArcadiaPieChart.razor.cs` — drill-down
- `arcadia-charts.css` — selection rect, print media, indicator lines

## Files to Create
- `Core/ChartSyncService.cs` — shared crosshair/zoom state
- `Core/Indicators/` — EMA, SMA, BollingerBands, MACD, RSI calculators
- `Core/IndicatorConfig.cs` — indicator configuration model
- `Core/DrillDownEventArgs.cs` — drill-down event model

## Tests
- Sync: hover chart A, verify crosshair appears on chart B
- Selection zoom: drag rect, verify new axis range
- EMA indicator: verify calculated values match known formula
- Drill-down: click bar, verify sub-data renders
- Print: verify print CSS applied

## Acceptance
- [ ] Synchronized crosshair across chart group
- [ ] Rubber band zoom with reset
- [ ] At least 3 financial indicators (EMA, SMA, Bollinger)
- [ ] Drill-down on bar and pie charts
- [ ] Multiple X-axes
- [ ] Print support
- [ ] All existing tests pass
