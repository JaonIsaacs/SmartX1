namespace SmartX1._1.Models;

/// <summary>
/// Represents a hierarchical device deployment node for recursive validation.
/// Used to validate nested configuration: Sub-Zone B -> Zone 1 -> Facility A
/// </summary>
public class DeploymentNode
{
    public string NodeId { get; set; } = string.Empty;
    public string NodeName { get; set; } = string.Empty;
    public string NodeType { get; set; } = string.Empty; // SubZone, Zone, Facility, Building, etc.
    public int Depth { get; set; }

    /// <summary>
    /// Child nodes (recursive structure for hierarchies).
    /// </summary>
    public List<DeploymentNode>? Children { get; set; }

    /// <summary>
    /// Associated sensors at this node level.
    /// </summary>
    public List<string> AssociatedSensorIds { get; set; } = new();

    public DeploymentNode() { }

    public DeploymentNode(string nodeId, string nodeName, string nodeType)
    {
        NodeId = nodeId;
        NodeName = nodeName;
        NodeType = nodeType;
    }
}