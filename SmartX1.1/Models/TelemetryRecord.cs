
namespace SmartX1._1.Models;

/// <summary>
/// A stored telemetry reading ingested from a device
/// </summary>
public class TelemetryRecord
{
    public string DeviceId { get; set; } = string.Empty;

    public SensorCategory Category { get; set; }

    public string DeploymentLocation { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;

    public string Unit { get; set; } = string.Empty;

    public DateTime CollectionTime { get; set; } = DateTime.UtcNow;

    public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
}