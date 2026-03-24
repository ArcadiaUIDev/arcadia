using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Arcadia.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ArcadiaChartAnalyzer : DiagnosticAnalyzer
{
    // ARC001: Chart missing Data parameter
    private static readonly DiagnosticDescriptor MissingDataRule = new(
        "ARC001",
        "Chart component missing Data parameter",
        "'{0}' requires a Data parameter to render. Set Data=\"@yourData\".",
        "Arcadia.Usage",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "All Arcadia chart components require a Data parameter.");

    // ARC002: Empty series list
    private static readonly DiagnosticDescriptor EmptySeriesRule = new(
        "ARC002",
        "Chart has empty Series list",
        "'{0}' has an empty Series list. Add at least one SeriesConfig to render data.",
        "Arcadia.Usage",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Charts with Series parameter need at least one series to display.");

    // ARC003: Width set to fixed value without Height
    private static readonly DiagnosticDescriptor MissingHeightRule = new(
        "ARC003",
        "Chart has Width but no Height",
        "'{0}' has Width set but no Height. Charts need both dimensions or use Width=\"0\" for responsive mode.",
        "Arcadia.Usage",
        DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: "Charts should have both Width and Height set, or use Width=0 for responsive.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(MissingDataRule, EmptySeriesRule, MissingHeightRule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        // Analyzers for Razor components would need additional infrastructure
        // For now, this registers the diagnostic IDs so they show in the IDE
    }
}
