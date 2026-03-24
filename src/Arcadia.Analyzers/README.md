# Arcadia.Analyzers

Roslyn analyzers for Arcadia Controls — compile-time warnings for common chart configuration mistakes.

## Rules

| ID | Severity | Description |
|----|----------|-------------|
| ARC001 | Warning | Chart component missing Data parameter |
| ARC002 | Warning | Chart has empty Series list |
| ARC003 | Info | Chart has Width but no Height |

## Installation

```bash
dotnet add package Arcadia.Analyzers
```

Warnings appear automatically in Visual Studio, Rider, and `dotnet build` output.
