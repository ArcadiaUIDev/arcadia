namespace Arcadia.Charts.Core;

/// <summary>
/// Represents a node (entity) on the outer ring of a chord diagram.
/// </summary>
public class ChordNode
{
    /// <summary>Unique identifier for this node.</summary>
    public string Id { get; set; } = "";

    /// <summary>Display label for this node.</summary>
    public string Label { get; set; } = "";

    /// <summary>Optional color override. Null uses the chart palette.</summary>
    public string? Color { get; set; }
}

/// <summary>
/// Represents a connection (chord) between two nodes in a chord diagram.
/// </summary>
public class ChordLink
{
    /// <summary>The <see cref="ChordNode.Id"/> of the source node.</summary>
    public string SourceId { get; set; } = "";

    /// <summary>The <see cref="ChordNode.Id"/> of the target node.</summary>
    public string TargetId { get; set; } = "";

    /// <summary>The flow/relationship magnitude. Determines the chord ribbon width.</summary>
    public double Value { get; set; }

    /// <summary>Optional color override. Null uses the source node color at reduced opacity.</summary>
    public string? Color { get; set; }
}

/// <summary>
/// Event arguments for when a chord ribbon is clicked.
/// </summary>
public class ChordClickEventArgs
{
    /// <summary>The source node ID.</summary>
    public string SourceId { get; set; } = "";

    /// <summary>The target node ID.</summary>
    public string TargetId { get; set; } = "";

    /// <summary>The source node label.</summary>
    public string SourceLabel { get; set; } = "";

    /// <summary>The target node label.</summary>
    public string TargetLabel { get; set; } = "";

    /// <summary>The flow value.</summary>
    public double Value { get; set; }
}
