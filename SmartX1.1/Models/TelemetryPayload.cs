using SmartX1._1.Models;

namespace SmartX1._1.Models;

/// <summary>
/// Payload structure for incoming telemetry data from iot   devices
/// </summary>
public class TelemetryPayload
{
    /// <summary>
    /// Device identifier (MAC address or unique ID).
    /// </summary>
    public string DeviceId { get; set; } = string.Empty;

    /// <summary>
    /// Sensor value as string (will be converted to appropriate type).
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// Sensor category.
    /// </summary>
    public SensorCategory Category { get; set; }

    /// <summary>
    /// Deployment location.
    /// </summary>
    public string DeploymentLocation { get; set; } = string.Empty;

    /// <summary>
    /// Unit of measurement.
    /// </summary>
    public string Unit { get; set; } = string.Empty;

    /// <summary>
    /// Optional timestamp (defaults to server time if not provided).
    /// </summary>
    public DateTime? CollectionTime { get; set; }
}