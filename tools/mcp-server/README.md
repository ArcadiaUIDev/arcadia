# Arcadia Controls MCP Server

An [MCP (Model Context Protocol)](https://modelcontextprotocol.io) server that lets AI assistants generate Blazor chart code using Arcadia Controls components.

## What It Does

When connected to Claude Code, Claude Desktop, VS Code Copilot, or any MCP-compatible AI tool, this server provides 4 tools:

| Tool | Description |
|------|-------------|
| `generate_chart` | Generate ready-to-paste Blazor code for any of 12 chart types |
| `list_chart_types` | List all available charts with their capabilities |
| `get_chart_docs` | Get detailed API reference for a specific chart |
| `suggest_chart_type` | Describe your data and get chart type recommendations |

## Setup

### Claude Code

```bash
claude mcp add arcadia-controls node /path/to/helixui/tools/mcp-server/index.js
```

### Claude Desktop

Add to `claude_desktop_config.json`:

```json
{
  "mcpServers": {
    "arcadia-controls": {
      "command": "node",
      "args": ["/path/to/helixui/tools/mcp-server/index.js"]
    }
  }
}
```

## Example Usage

Ask your AI assistant:

> "Create a line chart showing monthly revenue vs target with smooth curves and area fill"

The `generate_chart` tool returns complete Blazor code:

```razor
<HelixLineChart TItem="DataRecord" Data="@_data"
                XField="@(d => (object)d.Label)" Series="@_series"
                Height="350" Width="0" ShowPoints="true"
                AnimateOnLoad="true" Title="Revenue vs Target" />

@code {
    record DataRecord(string Label, double Value, double Target);
    // ... complete data and series config
}
```

## Supported Chart Types

Line, Bar, Pie/Donut, Scatter/Bubble, Candlestick, Radar, Gauge, Heatmap, Funnel, Treemap, Waterfall, Rose/Polar
