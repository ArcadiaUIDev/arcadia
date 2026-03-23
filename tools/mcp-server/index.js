#!/usr/bin/env node

import { McpServer } from "@modelcontextprotocol/sdk/server/mcp.js";
import { StdioServerTransport } from "@modelcontextprotocol/sdk/server/stdio.js";
import { z } from "zod";
import { chartTemplates } from "./templates.js";
import { chartDocs } from "./docs.js";

const server = new McpServer({
  name: "arcadia-controls",
  version: "1.0.0",
});

// ── Tool: generate_chart ──────────────────────────────────
server.tool(
  "generate_chart",
  "Generate Blazor Razor code for an Arcadia chart component. Returns ready-to-paste code with data model, series config, and chart markup.",
  {
    chartType: z.enum([
      "line", "bar", "pie", "scatter", "candlestick", "radar",
      "gauge", "heatmap", "funnel", "treemap", "waterfall", "rose"
    ]).describe("The type of chart to generate"),
    title: z.string().optional().describe("Chart title"),
    dataDescription: z.string().describe("Describe the data shape, e.g. 'monthly revenue and target for 2026'"),
    features: z.array(z.enum([
      "area-fill", "stacked", "smooth-curves", "trendline", "annotations",
      "crosshair", "streaming", "data-labels", "donut", "bubble",
      "full-circle", "thresholds", "dashed", "combo", "legend"
    ])).optional().describe("Optional features to include"),
    seriesCount: z.number().optional().describe("Number of data series (for multi-series charts)"),
    width: z.number().optional().describe("Chart width in pixels (0 = responsive)"),
    height: z.number().optional().describe("Chart height in pixels"),
  },
  async ({ chartType, title, dataDescription, features = [], seriesCount = 1, width = 0, height = 350 }) => {
    const template = chartTemplates[chartType];
    if (!template) {
      return { content: [{ type: "text", text: `Unknown chart type: ${chartType}` }] };
    }

    const code = template.generate({
      title,
      dataDescription,
      features,
      seriesCount,
      width,
      height,
    });

    return {
      content: [{
        type: "text",
        text: `Here's the Blazor code for your ${chartType} chart:\n\n\`\`\`razor\n${code}\n\`\`\`\n\nMake sure you have the Arcadia.Charts NuGet package installed and the CSS linked.`
      }]
    };
  }
);

// ── Tool: list_chart_types ────────────────────────────────
server.tool(
  "list_chart_types",
  "List all available Arcadia chart types with their parameters and capabilities.",
  {},
  async () => {
    const summary = Object.entries(chartDocs).map(([type, doc]) => {
      return `### ${doc.name}\n- Component: \`${doc.component}\`\n- Use for: ${doc.useCase}\n- Key params: ${doc.keyParams.join(", ")}`;
    }).join("\n\n");

    return {
      content: [{
        type: "text",
        text: `# Arcadia Controls — 12 Chart Types\n\n${summary}`
      }]
    };
  }
);

// ── Tool: get_chart_docs ──────────────────────────────────
server.tool(
  "get_chart_docs",
  "Get detailed documentation and API reference for a specific Arcadia chart type.",
  {
    chartType: z.enum([
      "line", "bar", "pie", "scatter", "candlestick", "radar",
      "gauge", "heatmap", "funnel", "treemap", "waterfall", "rose"
    ]).describe("The chart type to get docs for"),
  },
  async ({ chartType }) => {
    const doc = chartDocs[chartType];
    if (!doc) {
      return { content: [{ type: "text", text: `No docs for: ${chartType}` }] };
    }

    return {
      content: [{
        type: "text",
        text: `# ${doc.name}\n\n${doc.description}\n\n## Component\n\`${doc.component}\`\n\n## Parameters\n${doc.parameters}\n\n## Example\n\`\`\`razor\n${doc.example}\n\`\`\``
      }]
    };
  }
);

// ── Tool: suggest_chart_type ──────────────────────────────
server.tool(
  "suggest_chart_type",
  "Given a data description, suggest the best Arcadia chart type and configuration.",
  {
    dataDescription: z.string().describe("Describe what data you have and what you want to show"),
  },
  async ({ dataDescription }) => {
    const desc = dataDescription.toLowerCase();

    let suggestions = [];

    if (desc.includes("time") || desc.includes("trend") || desc.includes("month") || desc.includes("over time"))
      suggestions.push({ type: "line", reason: "Time series data is best shown as a line chart", features: ["smooth-curves", "area-fill"] });

    if (desc.includes("compar") || desc.includes("category") || desc.includes("by region"))
      suggestions.push({ type: "bar", reason: "Categorical comparisons work well as bar charts", features: ["legend"] });

    if (desc.includes("proportion") || desc.includes("share") || desc.includes("percentage") || desc.includes("budget"))
      suggestions.push({ type: "pie", reason: "Part-of-whole data suits a pie/donut chart", features: ["donut"] });

    if (desc.includes("correlation") || desc.includes("scatter") || desc.includes("x and y") || desc.includes("distribution"))
      suggestions.push({ type: "scatter", reason: "Two-variable relationships are clear in scatter plots", features: desc.includes("size") ? ["bubble"] : [] });

    if (desc.includes("stock") || desc.includes("ohlc") || desc.includes("candlestick") || desc.includes("price"))
      suggestions.push({ type: "candlestick", reason: "Financial OHLC data needs a candlestick chart", features: [] });

    if (desc.includes("skill") || desc.includes("radar") || desc.includes("multi-dimension"))
      suggestions.push({ type: "radar", reason: "Multi-dimensional comparisons suit radar charts", features: [] });

    if (desc.includes("kpi") || desc.includes("gauge") || desc.includes("metric") || desc.includes("progress"))
      suggestions.push({ type: "gauge", reason: "Single KPI values are best as gauges", features: ["thresholds"] });

    if (desc.includes("heat") || desc.includes("density") || desc.includes("grid") || desc.includes("frequency"))
      suggestions.push({ type: "heatmap", reason: "2D frequency/density data works as a heatmap", features: [] });

    if (desc.includes("funnel") || desc.includes("conversion") || desc.includes("pipeline"))
      suggestions.push({ type: "funnel", reason: "Sequential stage data fits a funnel chart", features: [] });

    if (desc.includes("hierarchy") || desc.includes("treemap") || desc.includes("allocation"))
      suggestions.push({ type: "treemap", reason: "Hierarchical proportional data suits a treemap", features: [] });

    if (desc.includes("waterfall") || desc.includes("cumulative") || desc.includes("breakdown"))
      suggestions.push({ type: "waterfall", reason: "Cumulative gain/loss data fits a waterfall chart", features: [] });

    if (desc.includes("wind") || desc.includes("direction") || desc.includes("cyclical") || desc.includes("polar"))
      suggestions.push({ type: "rose", reason: "Cyclical/directional data is ideal for a rose chart", features: [] });

    if (desc.includes("live") || desc.includes("real-time") || desc.includes("stream"))
      suggestions.push({ type: "line", reason: "Real-time data uses a streaming line chart", features: ["streaming"] });

    if (suggestions.length === 0)
      suggestions.push({ type: "line", reason: "Line chart is a good default for most data", features: [] });

    const text = suggestions.map((s, i) =>
      `${i + 1}. **${s.type.charAt(0).toUpperCase() + s.type.slice(1)} Chart** — ${s.reason}${s.features.length ? `\n   Recommended features: ${s.features.join(", ")}` : ""}`
    ).join("\n\n");

    return {
      content: [{
        type: "text",
        text: `# Suggested Chart Types\n\nBased on: "${dataDescription}"\n\n${text}\n\nUse \`generate_chart\` to create the code for any of these.`
      }]
    };
  }
);

// ── Start ─────────────────────────────────────────────────
const transport = new StdioServerTransport();
await server.connect(transport);
