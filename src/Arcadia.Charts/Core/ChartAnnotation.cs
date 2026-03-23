namespace Arcadia.Charts.Core;

/// <summary>
/// Defines an annotation marker on a chart — a label or callout at a specific data point.
/// </summary>
public class ChartAnnotation
{
    /// <summary>X-axis index or category position.</summary>
    public int DataIndex { get; set; }

    /// <summary>Y value for the annotation marker.</summary>
    public double? YValue { get; set; }

    /// <summary>Label text to display.</summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>Color for the marker and label.</summary>
    public string Color { get; set; } = "var(--arcadia-color-danger)";

    /// <summary>Whether to show a vertical line at this point.</summary>
    public bool ShowLine { get; set; } = true;
}
