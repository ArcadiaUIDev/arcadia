namespace Arcadia.DataGrid.Core;

/// <summary>
/// Selection behavior for the data grid.
/// </summary>
public enum DataGridSelectionMode
{
    /// <summary>No row selection.</summary>
    None,

    /// <summary>Click to select a single row at a time.</summary>
    SingleRow,

    /// <summary>Checkbox column with select-all header for multi-row selection.</summary>
    Multiple
}
