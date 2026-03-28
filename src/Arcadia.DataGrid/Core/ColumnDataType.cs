namespace Arcadia.DataGrid.Components;

/// <summary>
/// Auto-detected data type for a grid column. Determines filter input rendering.
/// </summary>
internal enum ColumnDataType
{
    /// <summary>Text — renders text input with operator dropdown.</summary>
    String,
    /// <summary>Numeric — renders number input with operator dropdown.</summary>
    Number,
    /// <summary>Boolean — renders tri-state dropdown (All/True/False).</summary>
    Boolean,
    /// <summary>Date — renders date picker input.</summary>
    Date
}
