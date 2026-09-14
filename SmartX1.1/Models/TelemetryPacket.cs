namespace SmartX1._1.Models;

/// <summary>
/// Generic wrapper for handling disparate sensor data types uniformly
/// Supports float , int ,, bool  without boxing/unboxing overhead
/// </summary>
public class TelemetryPacket<T> where T : struct, IComparable, IFormattable, IConvertible
{
    /// <summary>
    /// The actual sensor value.
    /// </summary>
    public T Value { get; set; }

    /// <summary>
    /// Device identifier (MAC address or unique sensor ID).
    /// </summary>
    public string DeviceId { get; set; } = string.Empty;

    /// <summary>
    /// Sensor category (e.g., Temperature, Power, Switch).
    /// </summary>
    public SensorCategory Category { get; set; }

    /// <summary>
    /// Deployment location (e.g., Room, Zone, Node ID).
    /// </summary>
    public string DeploymentLocation { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp when data was collected.
    /// </summary>
    public DateTime CollectionTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Unit of measurement (e.g., °C, W, state).
    /// </summary>
    public string Unit { get; set; } = string.Empty;

    public TelemetryPacket() { }

    public TelemetryPacket(T value, string deviceId, SensorCategory category, string location, string unit = "")
    {
        Value = value;
        DeviceId = deviceId;
        Category = category;
        DeploymentLocation = location;
        Unit = unit;
        CollectionTime = DateTime.UtcNow;
    }

    /// <summary>
    /// Overload + operator for aggregating sensor values (e.g., Meter3 = Meter1 + Meter2).
    /// </summary>
    public static TelemetryPacket<T> operator +(TelemetryPacket<T> left, TelemetryPacket<T> right)
    {
        if (left == null || right == null)
            throw new ArgumentNullException("Operands cannot be null");

        dynamic leftVal = left.Value;
        dynamic rightVal = right.Value;

        return new TelemetryPacket<T>
        {
            Value = (T)(leftVal + rightVal),
            DeviceId = $"{left.DeviceId}+{right.DeviceId}",
            Category = left.Category,
            DeploymentLocation = $"{left.DeploymentLocation}→{right.DeploymentLocation}",
            Unit = left.Unit,
            CollectionTime = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Overload - operator for calculating deltas between readings.
    /// </summary>
    public static TelemetryPacket<T> operator -(TelemetryPacket<T> left, TelemetryPacket<T> right)
    {
        if (left == null || right == null)
            throw new ArgumentNullException("Operands cannot be null");

        dynamic leftVal = left.Value;
        dynamic rightVal = right.Value;

        return new TelemetryPacket<T>
        {
            Value = (T)(leftVal - rightVal),
            DeviceId = $"{left.DeviceId}-{right.DeviceId}",
            Category = left.Category,
            DeploymentLocation = left.DeploymentLocation,
            Unit = left.Unit,
            CollectionTime = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Overload == operator for logical comparison of sensor values.
    /// </summary>
    public static bool operator ==(TelemetryPacket<T> left, TelemetryPacket<T> right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null) return false;
        return left.Value.CompareTo(right.Value) == 0;
    }

    /// <summary>
    /// Overload != operator for logical comparison of sensor values.
    /// </summary>
    public static bool operator !=(TelemetryPacket<T> left, TelemetryPacket<T> right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Overload > operator for sensor value comparison.
    /// </summary>
    public static bool operator >(TelemetryPacket<T> left, TelemetryPacket<T> right)
    {
        if (left == null || right == null)
            throw new ArgumentNullException("Operands cannot be null");
        return left.Value.CompareTo(right.Value) > 0;
    }

    /// <summary>
    /// Overload < operator for sensor value comparison.
    /// </summary>
    public static bool operator <(TelemetryPacket<T> left, TelemetryPacket<T> right)
    {
        if (left == null || right == null)
            throw new ArgumentNullException("Operands cannot be null");
        return left.Value.CompareTo(right.Value) < 0;
    }

    public override bool Equals(object? obj)
    {
        return obj is TelemetryPacket<T> packet && packet.Value.Equals(Value);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override string ToString()
    {
        return $"{Category} | Device: {DeviceId} | Location: {DeploymentLocation} | Value: {Value} {Unit} | Time: {CollectionTime:O}";
    }
}

public enum SensorCategory
{
    Environmental,    // Temperature, humidity, etc.
    PowerConsumption, // Watts, energy usage
    Actuator,         // Smart switches, valves
    Other
}