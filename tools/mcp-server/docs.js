export const chartDocs = {
  line: {
    name: "Line Chart",
    component: "ArcadiaLineChart<T>",
    useCase: "Time series, trends, multi-series comparisons",
    keyParams: ["XField", "Series", "ShowPoints", "ShowCrosshair", "NullHandling", "SlidingWindow", "Stacked"],
    description: "Multi-series line and area chart with smooth curves, trendlines, annotations, crosshair, streaming data, and null handling. Supports combo mode (bar + line on same axes).",
    parameters: `| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| XField | Func<T, object> | required | X-axis label extractor |
| Series | List<SeriesConfig<T>> | required | Series definitions |
| ShowPoints | bool | true | Show data point dots |
| ShowCrosshair | bool | false | Vertical tracking crosshair |
| NullHandling | NullHandling | Gap | Gap, Connect, or Zero for missing data |
| SlidingWindow | int | 0 | Max points for streaming (0 = unlimited) |
| Stacked | bool | false | Stack area series |
| Annotations | List<ChartAnnotation> | null | Event markers |`,
    example: `<ArcadiaLineChart TItem="SalesRecord" Data="@data"
                XField="@(d => (object)d.Month)"
                Series="@series"
                Height="350" Width="0" ShowPoints="true" />`
  },
  bar: {
    name: "Bar Chart",
    component: "ArcadiaBarChart<T>",
    useCase: "Categorical comparisons, grouped or stacked",
    keyParams: ["XField", "Series", "Stacked", "Rounded", "CornerRadius"],
    description: "Grouped or stacked bar chart with rounded corners, animations, and tooltips.",
    parameters: `| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| XField | Func<T, object> | required | Category label extractor |
| Series | List<SeriesConfig<T>> | required | Series definitions |
| Stacked | bool | false | Stack bars |
| Rounded | bool | true | Rounded bar corners |
| CornerRadius | double | 3 | Corner radius in pixels |`,
    example: `<ArcadiaBarChart TItem="QuarterlySales" Data="@data"
               XField="@(d => (object)d.Quarter)"
               Series="@series" Height="350" Width="0" />`
  },
  pie: {
    name: "Pie & Donut Chart",
    component: "ArcadiaPieChart<T>",
    useCase: "Part-of-whole proportions, budget breakdowns",
    keyParams: ["NameField", "ValueField", "InnerRadius", "LabelFormat", "PaddingAngle"],
    description: "Pie and donut chart with configurable labels, padding between slices, and multiple label formats.",
    parameters: `| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| NameField | Func<T, string> | required | Category name extractor |
| ValueField | Func<T, double> | required | Value extractor |
| InnerRadius | double | 0 | Set > 0 for donut mode |
| LabelFormat | PieLabelFormat | Percent | Label format (Percent, Name, Value, NamePercent, None) |
| PaddingAngle | double | 0 | Gap between slices in degrees |`,
    example: `<ArcadiaPieChart TItem="BudgetItem" Data="@data"
               NameField="@(d => d.Category)"
               ValueField="@(d => d.Amount)"
               Height="350" Width="350" InnerRadius="70" />`
  },
  scatter: {
    name: "Scatter / Bubble Chart",
    component: "ArcadiaScatterChart<T>",
    useCase: "Correlations, distributions, bubble sizing",
    keyParams: ["XField", "YField", "SizeField", "PointSize", "Color"],
    description: "Scatter chart with optional bubble sizing (SizeField) and trendlines.",
    parameters: `| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| XField | Func<T, double> | required | X-value extractor |
| YField | Func<T, double> | required | Y-value extractor |
| SizeField | Func<T, double>? | null | Bubble size extractor |
| PointSize | double | 5 | Default point radius |
| Color | string | null | Point color |`,
    example: `<ArcadiaScatterChart TItem="DataXY" Data="@data"
                   XField="@(d => d.X)" YField="@(d => d.Y)"
                   SizeField="@(d => d.Size)"
                   Height="400" Width="0" Color="primary" />`
  },
  candlestick: {
    name: "Candlestick (OHLC)",
    component: "ArcadiaCandlestickChart<T>",
    useCase: "Financial price data with open/high/low/close",
    keyParams: ["LabelField", "OpenField", "HighField", "LowField", "CloseField", "OverlaySeries"],
    description: "Financial OHLC candlestick chart with optional moving average overlay series.",
    parameters: `| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| LabelField | Func<T, string> | required | Date/label extractor |
| OpenField | Func<T, double> | required | Open price extractor |
| HighField | Func<T, double> | required | High price extractor |
| LowField | Func<T, double> | required | Low price extractor |
| CloseField | Func<T, double> | required | Close price extractor |
| OverlaySeries | List<SeriesConfig<T>>? | null | Line overlays (e.g. moving averages) |
| UpColor | string | green | Color for up candles |
| DownColor | string | red | Color for down candles |`,
    example: `<ArcadiaCandlestickChart TItem="StockData" Data="@data"
                       LabelField="@(d => d.Date)"
                       OpenField="@(d => d.Open)" HighField="@(d => d.High)"
                       LowField="@(d => d.Low)" CloseField="@(d => d.Close)"
                       Height="420" Width="0" />`
  },
  radar: {
    name: "Radar / Spider Chart",
    component: "ArcadiaRadarChart<T>",
    useCase: "Multi-dimensional comparisons (skills, profiles)",
    keyParams: ["LabelField", "Series", "GridRings", "ShowFill", "ShowPoints"],
    description: "Multi-series radar chart with grid rings, fill, and vertex points.",
    parameters: `| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| LabelField | Func<T, string> | required | Axis label extractor |
| Series | List<SeriesConfig<T>> | required | Series definitions |
| GridRings | int | 5 | Number of grid rings |
| ShowFill | bool | true | Fill polygon area |
| ShowPoints | bool | true | Show vertex dots |`,
    example: `<ArcadiaRadarChart TItem="SkillProfile" Data="@data"
                 LabelField="@(d => d.Skill)" Series="@series"
                 Height="400" Width="400" />`
  },
  gauge: {
    name: "Gauge Chart",
    component: "ArcadiaGaugeChart",
    useCase: "Single KPI values, progress, health monitors",
    keyParams: ["Value", "Min", "Max", "Label", "Thresholds", "FullCircle"],
    description: "Radial gauge with color thresholds, semi-circle or full-circle mode.",
    parameters: `| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| Value | double | required | Current value |
| Min | double | 0 | Range minimum |
| Max | double | 100 | Range maximum |
| Label | string | null | Label below value |
| Thresholds | List<GaugeThreshold> | null | Color thresholds |
| FullCircle | bool | false | Full 360° mode |
| ValueFormatString | string | null | Format string |`,
    example: `<ArcadiaGaugeChart Value="73" Min="0" Max="100"
                 Label="CPU" Width="200" Height="160"
                 Thresholds="@thresholds" />`
  },
  heatmap: {
    name: "Heatmap",
    component: "ArcadiaHeatmap<T>",
    useCase: "2D frequency, density, or correlation grids",
    keyParams: ["XField", "YField", "ValueField", "LowColor", "HighColor", "ShowValues"],
    description: "2D color grid with customizable color scale and cell value display.",
    parameters: `| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| XField | Func<T, string> | required | Column category extractor |
| YField | Func<T, string> | required | Row category extractor |
| ValueField | Func<T, double> | required | Cell value extractor |
| LowColor | string | #f1f5f9 | Low end of color scale |
| HighColor | string | #2563eb | High end of color scale |
| ShowValues | bool | false | Show values in cells |`,
    example: `<ArcadiaHeatmap TItem="HeatCell" Data="@data"
              XField="@(d => d.Day)" YField="@(d => d.Hour)"
              ValueField="@(d => d.Count)" Height="320" Width="0" />`
  },
  funnel: {
    name: "Funnel Chart",
    component: "ArcadiaFunnelChart<T>",
    useCase: "Conversion funnels, sales pipelines",
    keyParams: ["NameField", "ValueField"],
    description: "Conversion funnel with progressively narrowing stages.",
    parameters: `| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| NameField | Func<T, string> | required | Stage name extractor |
| ValueField | Func<T, double> | required | Stage value extractor |`,
    example: `<ArcadiaFunnelChart TItem="FunnelStage" Data="@data"
                  NameField="@(d => d.Stage)"
                  ValueField="@(d => d.Count)"
                  Height="400" Width="500" />`
  },
  treemap: {
    name: "Treemap Chart",
    component: "ArcadiaTreemapChart<T>",
    useCase: "Hierarchical proportional data, budget allocations",
    keyParams: ["NameField", "ValueField"],
    description: "Nested rectangles with area proportional to value. Squarified layout.",
    parameters: `| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| NameField | Func<T, string> | required | Category name extractor |
| ValueField | Func<T, double> | required | Value extractor (determines area) |`,
    example: `<ArcadiaTreemapChart TItem="Dept" Data="@data"
                   NameField="@(d => d.Name)"
                   ValueField="@(d => d.Budget)"
                   Height="400" Width="600" />`
  },
  waterfall: {
    name: "Waterfall Chart",
    component: "ArcadiaWaterfallChart<T>",
    useCase: "Cumulative gain/loss, financial breakdowns",
    keyParams: ["CategoryField", "ValueField", "PositiveColor", "NegativeColor"],
    description: "Shows cumulative effect of sequential positive and negative values.",
    parameters: `| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| CategoryField | Func<T, string> | required | Category label extractor |
| ValueField | Func<T, double> | required | Value (positive = increase, negative = decrease) |
| PositiveColor | string | green | Color for positive bars |
| NegativeColor | string | red | Color for negative bars |`,
    example: `<ArcadiaWaterfallChart TItem="FlowItem" Data="@data"
                     CategoryField="@(d => d.Label)"
                     ValueField="@(d => d.Amount)"
                     Height="400" Width="600" />`
  },
  rose: {
    name: "Rose / Polar Area Chart",
    component: "ArcadiaRoseChart<T>",
    useCase: "Cyclical data, wind roses, directional patterns",
    keyParams: ["NameField", "ValueField", "ShowLabels"],
    description: "Equal-angle sectors with value-proportional radii. Ideal for directional or cyclical data.",
    parameters: `| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| NameField | Func<T, string> | required | Category name extractor |
| ValueField | Func<T, double> | required | Value extractor (determines radius) |
| ShowLabels | bool | true | Show labels on sectors |`,
    example: `<ArcadiaRoseChart TItem="WindData" Data="@data"
                NameField="@(d => d.Direction)"
                ValueField="@(d => d.Speed)"
                Height="400" Width="400" />`
  },
};
