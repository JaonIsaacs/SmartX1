using SmartX1._1.Models;

namespace SmartX.Models;

/// <summary>
/// Represents a registered sensor device with its metadata and configuration.
/// </summary>
public class SensorProfile
{
    public int Id { get; set; }

    /// <summary>
    /// Device MAC address or unique identifier (e.g., ESP32:AABBCCDDEE00).
    /// </summary>
    public string DeviceId { get; set; } = string.Empty;

    /// <summary>
    /// Human-readable sensor name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Sensor category (Environmental, PowerConsumption, Actuator, etc.).
    /// </summary>
    public SensorCategory Category { get; set; }

    /// <summary>
    /// Deployment location hierarchy (e.g., "Sub-Zone B > Zone 1 > Facility A").
    /// </summary>
    public string DeploymentLocation { get; set; } = string.Empty;

    /// <summary>
    /// Unit of measurement for this sensor.
    /// </summary>
    public string Unit { get; set; } = string.Empty;

    /// <summary>
    /// Data type expected from this sensor (float, int, bool, etc.).
    /// </summary>
    public string DataType { get; set; } = "float";

    /// <summary>
    /// Date when this sensor was registered.
    /// </summary>
    public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Whether this sensor is currently active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Last time this sensor sent data.
    /// </summary>
    public DateTime? LastDataPoint { get; set; }

    /// <summary>
    /// Hardware configuration file reference (JSON blob for complex configs).
    /// </summary>
    public string? ConfigurationJson { get; set; }
}