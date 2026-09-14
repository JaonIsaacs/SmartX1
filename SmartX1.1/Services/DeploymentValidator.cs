using SmartX1._1.Models;

namespace SmartX1._1.Models;

/// <summary>
/// Recursive validation algorithm for nested device deployment hierarchies.
/// Validates that sensors are safely configured within Sub-Zone -> Zone -> Facility hierarchy.
/// </summary>
public class DeploymentValidator
{
    /// <summary>
    /// Recursively validates a device deployment tree.
    /// Ensures no sensor is placed in an invalid location hierarchy.
    /// </summary>
    public bool ValidateDeploymentTree(DeploymentNode node, int maxDepth = 5, List<string>? errors = null)
    {
        errors ??= new List<string>();

        /// Base validation
        if (node == null)
        {
            errors.Add("Node cannot be null");
            return false;
        }

        if (string.IsNullOrWhiteSpace(node.NodeId))
        {
            errors.Add("Node ID is required");
            return false;
        }

        /// Depth validation
        if (node.Depth > maxDepth)
        {
            errors.Add($"Node '{node.NodeId}' exceeds maximum depth of {maxDepth}");
            return false;
        }

        /// Validate node type hierarchy
        if (!IsValidNodeType(node.NodeType))
        {
            errors.Add($"Invalid node type: {node.NodeType}");
            return false;
        }

        /// Validate associated sensors
        foreach (var sensorId in node.AssociatedSensorIds)
        {
            if (string.IsNullOrWhiteSpace(sensorId))
            {
                errors.Add($"Empty sensor ID in node '{node.NodeId}'");
                return false;
            }
        }

        /// Recursively validate children
        if (node.Children != null && node.Children.Count > 0)
        {
            foreach (var child in node.Children)
            {
                child.Depth = node.Depth + 1;

                if (!ValidateDeploymentTree(child, maxDepth, errors))
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Recursively finds a sensor by its ID in the deployment tree.
    /// </summary>
    public DeploymentNode? FindSensorLocation(DeploymentNode node, string sensorId)
    {
        if (node == null)
            return null;

        /// Check current node
        if (node.AssociatedSensorIds.Contains(sensorId))
            return node;

        /// Recursively check children
        if (node.Children != null)
        {
            foreach (var child in node.Children)
            {
                var result = FindSensorLocation(child, sensorId);
                if (result != null)
                    return result;
            }
        }

        return null;
    }

    /// <summary>
    /// Recursively counts all sensors in the deployment tree.
    /// </summary>
    public int CountTotalSensors(DeploymentNode node)
    {
        if (node == null)
            return 0;

        int count = node.AssociatedSensorIds.Count;

        if (node.Children != null)
        {
            foreach (var child in node.Children)
            {
                count += CountTotalSensors(child);
            }
        }

        return count;
    }

    /// <summary>
    /// Gets the full hierarchical path to a deployment node
    
    /// </summary>
    public string GetNodePath(DeploymentNode node, DeploymentNode? root = null)
    {
        if (node == null)
            return string.Empty;

        var path = new List<string> { node.NodeName };

        /// Walk up the tree to find all parent nodes
        var current = root;
        if (current != null)
        {
            CollectPath(current, node.NodeId, path);
        }

        path.Reverse();
        return string.Join(" > ", path);
    }

    private bool CollectPath(DeploymentNode current, string targetId, List<string> path)
    {
        if (current.NodeId == targetId)
            return true;

        if (current.Children != null)
        {
            foreach (var child in current.Children)
            {
                if (CollectPath(child, targetId, path))
                {
                    path.Add(current.NodeName);
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Valid hierarchy levels for deployment nodes
    /// </summary>
    private static readonly HashSet<string> ValidNodeTypes = new()
    {
        "SubZone", "Zone", "Facility", "Building", "Floor", "Room", "Area"
    };

    private bool IsValidNodeType(string nodeType)
    {
        return ValidNodeTypes.Contains(nodeType);
    }
}