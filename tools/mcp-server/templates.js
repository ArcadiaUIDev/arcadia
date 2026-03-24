// Chart code generation templates for the MCP server

function makeRecord(chartType, dataDescription) {
  // Generate a reasonable record type based on the chart type
  switch (chartType) {
    case "line":
    case "bar":
      return { name: "DataRecord", fields: "string Label, double Value, double Target", sample: 'new("Jan", 100, 90), new("Feb", 120, 95), new("Mar", 110, 100), new("Apr", 140, 105)' };
    case "pie":
      return { name: "SliceData", fields: "string Name, double Value", sample: 'new("Category A", 40), new("Category B", 30), new("Category C", 20), new("Category D", 10)' };
    case "scatter":
      return { name: "PointData", fields: "double X, double Y, double Size", sample: 'new(10, 25, 5), new(20, 45, 8), new(30, 35, 12), new(40, 55, 6), new(50, 40, 15)' };
    case "candlestick":
      return { name: "OhlcData", fields: "string Date, double Open, double High, double Low, double Close", sample: 'new("Mon", 100, 110, 95, 105), new("Tue", 105, 115, 100, 112), new("Wed", 112, 120, 108, 108)' };
    case "radar":
      return { name: "AxisData", fields: "string Axis, double SeriesA, double SeriesB", sample: 'new("Speed", 80, 60), new("Power", 70, 90), new("Agility", 90, 50), new("Defense", 50, 70), new("Stamina", 60, 80)' };
    case "gauge":
      return null; // Gauge doesn't use generic data
    case "heatmap":
      return { name: "CellData", fields: "string X, string Y, double Value", sample: 'new("Mon", "9am", 5), new("Mon", "10am", 8), new("Tue", "9am", 3), new("Tue", "10am", 12)' };
    case "funnel":
      return { name: "StageData", fields: "string Stage, double Count", sample: 'new("Visitors", 10000), new("Signups", 4200), new("Trials", 1800), new("Paid", 720)' };
    case "treemap":
      return { name: "TreeNode", fields: "string Name, double Value", sample: 'new("Engineering", 450), new("Marketing", 180), new("Sales", 220), new("HR", 80)' };
    case "waterfall":
      return { name: "FlowItem", fields: "string Category, double Amount", sample: 'new("Revenue", 500), new("COGS", -180), new("OpEx", -120), new("Tax", -40)' };
    case "rose":
      return { name: "RoseData", fields: "string Name, double Value", sample: 'new("N", 12), new("NE", 8), new("E", 15), new("SE", 10), new("S", 7), new("SW", 18), new("W", 14), new("NW", 9)' };
    default:
      return { name: "DataRecord", fields: "string Label, double Value", sample: 'new("A", 10), new("B", 20), new("C", 30)' };
  }
}

function generateLine({ title, features, seriesCount, width, height }) {
  const hasArea = features.includes("area-fill");
  const hasSmooth = features.includes("smooth-curves");
  const hasTrendline = features.includes("trendline");
  const hasAnnotations = features.includes("annotations");
  const hasCrosshair = features.includes("crosshair");
  const hasStreaming = features.includes("streaming");
  const hasStacked = features.includes("stacked");
  const hasLabels = features.includes("data-labels");

  let series = "";
  if (seriesCount === 1) {
    series = `    List<SeriesConfig<DataRecord>> _series = new()
    {
        new() { Name = "Values", Field = d => d.Value, Color = "success"${hasArea ? ', ShowArea = true, AreaOpacity = 0.12' : ''}${hasSmooth ? ', CurveType = "smooth"' : ''}${hasTrendline ? ',\n                Trendline = new() { Type = TrendlineType.Linear, Color = "var(--arcadia-color-danger)", Dashed = true }' : ''} },
    };`;
  } else {
    series = `    List<SeriesConfig<DataRecord>> _series = new()
    {
        new() { Name = "Actual", Field = d => d.Value, Color = "success"${hasArea ? ', ShowArea = true, AreaOpacity = 0.12' : ''}${hasSmooth ? ', CurveType = "smooth"' : ''}, StrokeWidth = 2.5 },
        new() { Name = "Target", Field = d => d.Target, Color = "warning", Dashed = true, StrokeWidth = 2${hasSmooth ? ', CurveType = "smooth"' : ''} },
    };`;
  }

  let annotations = "";
  if (hasAnnotations) {
    annotations = `\n    List<ChartAnnotation> _annotations = new()
    {
        new() { DataIndex = 2, Label = "Event", Color = "var(--arcadia-color-info)" },
    };\n`;
  }

  const chartAttrs = [
    `TItem="DataRecord" Data="@_data"`,
    `XField="@(d => (object)d.Label)" Series="@_series"`,
    `Height="${height}" Width="${width}"`,
    hasStacked ? `Stacked="true"` : "",
    hasCrosshair ? `ShowCrosshair="true"` : "",
    hasAnnotations ? `Annotations="@_annotations"` : "",
    hasLabels ? `ShowDataLabels="true"` : "",
    hasStreaming ? `SlidingWindow="20" AnimateOnLoad="false"` : `AnimateOnLoad="true"`,
    `ShowPoints="true"`,
    title ? `Title="${title}"` : "",
  ].filter(Boolean).join("\n                ");

  return `@using Arcadia.Charts.Core
@using Arcadia.Charts.Components.Charts

<ArcadiaLineChart ${chartAttrs} />

@code {
    record DataRecord(string Label, double Value, double Target);

    List<DataRecord> _data = new()
    {
        new("Jan", 45, 50), new("Feb", 52, 50), new("Mar", 48, 52),
        new("Apr", 61, 55), new("May", 55, 55), new("Jun", 67, 58),
    };

${series}
${annotations}}`;
}

function generateGauge({ title, features, height }) {
  const hasFullCircle = features.includes("full-circle");
  const hasThresholds = features.includes("thresholds");

  let thresholds = "";
  if (hasThresholds) {
    thresholds = `\n    List<GaugeThreshold> _thresholds = new()
    {
        new() { Value = 0, Color = "var(--arcadia-color-success)" },
        new() { Value = 60, Color = "var(--arcadia-color-warning)" },
        new() { Value = 85, Color = "var(--arcadia-color-danger)" },
    };\n`;
  }

  return `@using Arcadia.Charts.Core
@using Arcadia.Charts.Components.Charts

<ArcadiaGaugeChart Value="73" Min="0" Max="100"
                 Label="${title || 'Progress'}" Width="200" Height="${hasFullCircle ? 200 : 160}"
                 ${hasFullCircle ? 'FullCircle="true"' : ''}
                 ${hasThresholds ? 'Thresholds="@_thresholds"' : ''}
                 AnimateOnLoad="true" />

@code {${thresholds}}`;
}

function generateSimpleChart(type, component, fieldConfig, { title, features, width, height }) {
  const record = makeRecord(type);
  return `@using Arcadia.Charts.Components.Charts

<${component} TItem="${record.name}" Data="@_data"
${fieldConfig}
                ${title ? `Title="${title}"` : ''}
                Height="${height}" Width="${width}" AnimateOnLoad="true" />

@code {
    record ${record.name}(${record.fields});

    List<${record.name}> _data = new()
    {
        ${record.sample}
    };
}`;
}

export const chartTemplates = {
  line: { generate: generateLine },
  bar: {
    generate: (opts) => {
      const hasStacked = opts.features.includes("stacked");
      return `@using Arcadia.Charts.Core
@using Arcadia.Charts.Components.Charts

<ArcadiaBarChart TItem="DataRecord" Data="@_data"
               XField="@(d => (object)d.Label)" Series="@_series"
               Height="${opts.height}" Width="${opts.width}"
               ${hasStacked ? 'Stacked="true"' : ''}
               ${opts.title ? `Title="${opts.title}"` : ''}
               AnimateOnLoad="true" />

@code {
    record DataRecord(string Label, double ValueA, double ValueB);

    List<DataRecord> _data = new()
    {
        new("Q1", 145, 98), new("Q2", 183, 112),
        new("Q3", 215, 134), new("Q4", 268, 156),
    };

    List<SeriesConfig<DataRecord>> _series = new()
    {
        new() { Name = "Series A", Field = d => d.ValueA, Color = "primary" },
        new() { Name = "Series B", Field = d => d.ValueB, Color = "success" },
    };
}`;
    }
  },
  pie: {
    generate: (opts) => {
      const isDonut = opts.features.includes("donut");
      return generateSimpleChart("pie", "ArcadiaPieChart",
        `               NameField="@(d => d.Name)" ValueField="@(d => d.Value)"${isDonut ? '\n               InnerRadius="70"' : ''}`, opts);
    }
  },
  scatter: {
    generate: (opts) => {
      const isBubble = opts.features.includes("bubble");
      return generateSimpleChart("scatter", "ArcadiaScatterChart",
        `               XField="@(d => d.X)" YField="@(d => d.Y)"${isBubble ? '\n               SizeField="@(d => d.Size)"' : ''}\n               Color="primary" PointSize="5"`, opts);
    }
  },
  candlestick: {
    generate: (opts) => generateSimpleChart("candlestick", "ArcadiaCandlestickChart",
      `               LabelField="@(d => d.Date)"\n               OpenField="@(d => d.Open)" HighField="@(d => d.High)"\n               LowField="@(d => d.Low)" CloseField="@(d => d.Close)"`, opts)
  },
  radar: {
    generate: (opts) => `@using Arcadia.Charts.Core
@using Arcadia.Charts.Components.Charts

<ArcadiaRadarChart TItem="AxisData" Data="@_data"
                 LabelField="@(d => d.Axis)" Series="@_series"
                 Height="${opts.height}" Width="${opts.width}"
                 ${opts.title ? `Title="${opts.title}"` : ''}
                 AnimateOnLoad="true" />

@code {
    record AxisData(string Axis, double SeriesA, double SeriesB);

    List<AxisData> _data = new()
    {
        new("Speed", 80, 60), new("Power", 70, 90), new("Agility", 90, 50),
        new("Defense", 50, 70), new("Stamina", 60, 80),
    };

    List<SeriesConfig<AxisData>> _series = new()
    {
        new() { Name = "Player A", Field = d => d.SeriesA, Color = "primary" },
        new() { Name = "Player B", Field = d => d.SeriesB, Color = "success" },
    };
}`
  },
  gauge: { generate: generateGauge },
  heatmap: {
    generate: (opts) => generateSimpleChart("heatmap", "ArcadiaHeatmap",
      `              XField="@(d => d.X)" YField="@(d => d.Y)"\n              ValueField="@(d => d.Value)"`, opts)
  },
  funnel: {
    generate: (opts) => generateSimpleChart("funnel", "ArcadiaFunnelChart",
      `                  NameField="@(d => d.Stage)" ValueField="@(d => d.Count)"`, opts)
  },
  treemap: {
    generate: (opts) => generateSimpleChart("treemap", "ArcadiaTreemapChart",
      `                   NameField="@(d => d.Name)" ValueField="@(d => d.Value)"`, opts)
  },
  waterfall: {
    generate: (opts) => generateSimpleChart("waterfall", "ArcadiaWaterfallChart",
      `                     CategoryField="@(d => d.Category)" ValueField="@(d => d.Amount)"`, opts)
  },
  rose: {
    generate: (opts) => generateSimpleChart("rose", "ArcadiaRoseChart",
      `                NameField="@(d => d.Name)" ValueField="@(d => d.Value)"`, opts)
  },
};
