namespace Arcadia.Charts.Core;

/// <summary>
/// Represents a node (category or stage) in a Sankey diagram.
/// </summary>
public class SankeyNode
{
    /// <summary>Unique identifier for this node.</summary>
    public string Id { get; set; } = "";

    /// <summary>Display label for this node.</summary>
    public string Label { get; set; } = "";

    /// <summary>Optional color override. Null uses the chart palette.</summary>
    public string? Color { get; set; }
}

/// <summary>
/// Represents a flow link between two nodes in a Sankey diagram.
/// </summary>
public class SankeyLink
{
    /// <summary>The <see cref="SankeyNode.Id"/> of the source node.</summary>
    public string SourceId { get; set; } = "";

    /// <summary>The <see cref="SankeyNode.Id"/> of the target node.</summary>
    public string TargetId { get; set; } = "";

    /// <summary>The flow quantity. Determines the visual width of the link.</summary>
    public double Value { get; set; }

    /// <summary>Optional color override. Null uses the source node color at reduced opacity.</summary>
    public string? Color { get; set; }
}

/// <summary>
/// Event arguments for when a Sankey diagram link is clicked.
/// </summary>
public class SankeyLinkClickEventArgs
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
