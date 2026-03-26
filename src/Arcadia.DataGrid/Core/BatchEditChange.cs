namespace Arcadia.DataGrid.Core;

/// <summary>
/// Represents a single cell change in batch editing mode.
/// </summary>
/// <typeparam name="TItem">The row data type.</typeparam>
public class BatchEditChange<TItem>
{
    /// <summary>The row item that was modified.</summary>
    public TItem Item { get; set; } = default!;

    /// <summary>The column key of the modified cell.</summary>
    public string ColumnKey { get; set; } = "";

    /// <summary>The original cell value before editing.</summary>
    public object? OldValue { get; set; }

    /// <summary>The new cell value after editing.</summary>
    public object? NewValue { get; set; }
}
