namespace Arcadia.Charts.Core;

/// <summary>
/// Event arguments for chart point click events.
/// Contains the data item, its index, and the series index.
/// </summary>
public class PointClickEventArgs<T>
{
    /// <summary>The clicked data item.</summary>
    public T Item { get; set; } = default!;

    /// <summary>The index of the item in the Data list.</summary>
    public int DataIndex { get; set; }

    /// <summary>The series index (for multi-series charts). -1 for single-series charts.</summary>
    public int SeriesIndex { get; set; } = -1;

    /// <summary>The series name, if available.</summary>
    public string? SeriesName { get; set; }
}
