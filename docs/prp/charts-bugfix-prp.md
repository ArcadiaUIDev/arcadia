# Charts Bugfix PRP — Audit Response

## Source: Comprehensive audit found 31 issues (3 critical, 5 high, 8 medium)

## Phase 1 — Critical Fixes (do NOW)

### 1. LogScaleAdapter Method Hiding (Critical)
- `new` keyword on Scale/Invert creates virtual method hiding
- Fix: replace inheritance with composition — LogScaleAdapter wraps LogarithmicScale via delegation, not inheritance
- Or make LinearScale methods virtual

### 2. Secondary Y-Axis Scale Domain Mismatch (Critical)
- _y2Scale domain is pinned to tick min/max instead of actual y2Min/y2Max
- Fix: pass y2Min/y2Max directly to CreateYScale instead of tick-derived bounds

### 3. Missing Null Check for _yScale in Line Chart Template (Critical)
- Template accesses _yScale! without null check
- If OnParametersSet returns early, _yScale is null → NRE
- Fix: add `_yScale is not null` guard in template before SVG content

## Phase 2 — High Severity Fixes

### 4. Pan/Zoom JS Event Listener Leak
- DisablePanZoomAsync never called in DisposeAsync
- Fix: call `Interop.DisablePanZoomAsync(ContainerRef)` in DisposeAsync

### 5. ResizeObserver Callback After Disposal
- OnResized can fire after _disposed = true
- Fix: check _disposed at top of OnResized method

### 6. Stacked NaN Handling
- NaN values corrupt the entire stack
- Fix: skip NaN during stacking: `if (!double.IsNaN(v)) cumulativeMax[j] += v` (already there but verify in rendering)

### 7. TimeScale Invert Precision
- Cast to long truncates instead of rounding
- Fix: `new DateTime((long)Math.Round(_inner.Invert(pixel)))`

### 8. BandScale Unknown Category
- Returns RangeMin for unknown categories
- Fix: add warning/fallback that's more explicit

## Phase 3 — Medium Fixes

### 9. Chord Label Angular Wraparound
- Remove redundant first isLeftSide check, use only normalized angle

### 10. Path Recalculation Optimization
- Add change detection: track data hash, skip recalc if unchanged

### 11. Annotation DataIndex Bounds Check
- Guard against out-of-range DataIndex

### 12. Fix PointColorField Not Applied
- Use PointColorField in line chart circle rendering

## Not Fixing Now
- Layout engine title width estimate (low impact)
- Responsive tier exact boundaries (cosmetic)
- Legend position enforcement (feature scope)
- Streaming for non-line charts (feature scope)
