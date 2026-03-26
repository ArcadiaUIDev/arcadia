using System.Text.Json;

namespace Arcadia.DataGrid.Core;

/// <summary>
/// Serializable grid state for persistence (localStorage, server-side, etc.).
/// </summary>
public class DataGridState
{
    /// <summary>Column order (list of column keys in display order).</summary>
    public List<string>? ColumnOrder { get; set; }

    /// <summary>Column visibility (key → visible).</summary>
    public Dictionary<string, bool>? ColumnVisibility { get; set; }

    /// <summary>Column widths (key → CSS width string).</summary>
    public Dictionary<string, string>? ColumnWidths { get; set; }

    /// <summary>Current sort descriptors.</summary>
    public List<SortDescriptor>? Sorts { get; set; }

    /// <summary>Current filter descriptors.</summary>
    public List<FilterDescriptor>? Filters { get; set; }

    /// <summary>Current page size.</summary>
    public int? PageSize { get; set; }

    /// <summary>Serialize to JSON.</summary>
    public string ToJson() => JsonSerializer.Serialize(this);

    /// <summary>Deserialize from JSON.</summary>
    public static DataGridState? FromJson(string json)
    {
        try { return JsonSerializer.Deserialize<DataGridState>(json); }
        catch { return null; }
    }
}
