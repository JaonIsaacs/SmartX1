using SmartX.Models;

namespace SmartX1._1.Models;

/// <summary>
/// Represents one node in a deployment location hierarchy
/// (e.g., "Facility A" > "Zone 1" > "Sub-Zone B").
/// </summary>
public class LocationNode
{
    public string Name { get; set; } = string.Empty;
    public List<LocationNode> Children { get; set; } = new();
}

/// <summary>
/// Recursively validates and parses hierarchical deployment locations
/// (format: "Sub-Zone B > Zone 1 > Facility A").
/// Demonstrates recursion with a clear base case and depth guarding
/// against stack overflows from malformed/circular input.
/// </summary>
public static class LocationHierarchyValidator
{
    private const int MaxDepth = 20;

    /// <summary>
    /// Validates a "Child > Parent > Grandparent" style location string.
    /// Returns true if every segment is non-empty and depth is within bounds.
    /// </summary>
    public static bool IsValid(string deploymentLocation)
    {
        if (string.IsNullOrWhiteSpace(deploymentLocation))
            return false;

        var segments = deploymentLocation
            .Split('>', StringSplitOptions.TrimEntries)
            .ToArray();

        return ValidateSegments(segments, 0);
    }

    /// <summary>
    /// Base case: no segments left to check means everything above passed.
    /// Recursive case: validate the current segment, then recurse into the rest.
    /// </summary>
    private static bool ValidateSegments(string[] segments, int index)
    {
        /// Base case 1: reached the end successfully.
        if (index >= segments.Length)
            return true;

        /// Base case 2: guard against pathological/malformed input causing deep recursion.
        if (index > MaxDepth)
            return false;

        /// A segment must be non-empty and reasonably short.
        if (string.IsNullOrWhiteSpace(segments[index]) || segments[index].Length > 100)
            return false;

        // Recursive case: validate the remaining segments.
        return ValidateSegments(segments, index + 1);
    }

    /// <summary>
    /// Builds a tree of <see cref="LocationNode"/> from all registered sensors,
    /// recursively merging shared ancestors (e.g., multiple sensors under "Zone 1").
    /// </summary>
    public static LocationNode BuildHierarchyTree(IEnumerable<SensorProfile> sensors)
    {
        var root = new LocationNode { Name = "Root" };

        foreach (var sensor in sensors)
        {
            if (!IsValid(sensor.DeploymentLocation))
                continue;

            /// Reverse so the outermost facility becomes the top-level node.
            var segments = sensor.DeploymentLocation
                .Split('>', StringSplitOptions.TrimEntries)
                .Reverse()
                .ToArray();

            InsertPath(root, segments, 0);
        }

        return root;
    }

    /// <summary>
    /// Recursively walks/creates nodes for each path segment.
    /// </summary>
    private static void InsertPath(LocationNode current, string[] segments, int index)
    {
        // Base case: no more segments to insert.
        if (index >= segments.Length)
            return;

        var existing = current.Children.FirstOrDefault(c => c.Name == segments[index]);
        if (existing == null)
        {
            existing = new LocationNode { Name = segments[index] };
            current.Children.Add(existing);
        }

        /// Recursive case: descend into the next level.
        InsertPath(existing, segments, index + 1);
    }
}