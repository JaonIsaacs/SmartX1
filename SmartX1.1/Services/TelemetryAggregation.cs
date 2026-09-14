using SmartX1._1.Models;

namespace SmartX1._1.Models;

/// <summary>
/// Aggregates telemetry data using operator overloading.
/// Example: totalPower = meter1 + meter2 + meter3
/// Demonstrates advanced OOP with generic types and operator overloading.
/// </summary>
public class TelemetryAggregation
{
    /// <summary>
    /// Aggregates multiple sensor readings using the + operator.
    /// Useful for combining meter readings (e.g., total power = all meters summed).
    /// </summary>
    public TelemetryPacket<float> AggregatePowerMeters(params TelemetryPacket<float>[] meters)
    {
        if (meters == null || meters.Length == 0)
            throw new ArgumentException("At least one meter is required");

        var result = meters[0];

        for (int i = 1; i < meters.Length; i++)
        {
            result = result + meters[i];
        }

        return result;
    }

    /// <summary>
    /// Calculates delta between two readings 
    /// </summary>
    public TelemetryPacket<float> CalculateDelta(TelemetryPacket<float> current, TelemetryPacket<float> previous)
    {
        if (current == null || previous == null)
            throw new ArgumentNullException("Both readings are required");

        return current - previous;
    }

    /// <summary>
    /// Compares two sensor readings using operator overloading
    /// </summary>
    public bool IsThresholdExceeded(TelemetryPacket<float> reading, TelemetryPacket<float> threshold)
    {
        if (reading == null || threshold == null)
            throw new ArgumentNullException("Both values are required");

        return reading > threshold;
    }

    /// <summary>
    /// Checks if two readings are equal using == operator
    /// </summary>
    public bool AreReadingsEqual(TelemetryPacket<float> reading1, TelemetryPacket<float> reading2)
    {
        return reading1 == reading2;
    }

    /// <summary>
    /// Finds the average of multiple readings
    /// </summary>
    public TelemetryPacket<float> CalculateAverage(params TelemetryPacket<float>[] readings)
    {
        if (readings == null || readings.Length == 0)
            throw new ArgumentException("At least one reading is required");

        var sum = readings[0];
        for (int i = 1; i < readings.Length; i++)
        {
            sum = sum + readings[i];
        }

        dynamic sumValue = sum.Value;
        dynamic avgValue = (float)(sumValue / readings.Length);

        return new TelemetryPacket<float>
        {
            Value = avgValue,
            DeviceId = readings[0].DeviceId,
            Category = readings[0].Category,
            DeploymentLocation = readings[0].DeploymentLocation,
            Unit = readings[0].Unit
        };
    }

    /// <summary>
    /// Finds the maximum reading from a set of readings
    /// </summary>
    public TelemetryPacket<float> FindMaximum(params TelemetryPacket<float>[] readings)
    {
        if (readings == null || readings.Length == 0)
            throw new ArgumentException("At least one reading is required");

        var max = readings[0];
        for (int i = 1; i < readings.Length; i++)
        {
            if (readings[i] > max)
                max = readings[i];
        }

        return max;
    }

    /// <summary>
    /// Finds the minimum reading from a set of readings
    /// </summary>
    public TelemetryPacket<float> FindMinimum(params TelemetryPacket<float>[] readings)
    {
        if (readings == null || readings.Length == 0)
            throw new ArgumentException("At least one reading is required");

        var min = readings[0];
        for (int i = 1; i < readings.Length; i++)
        {
            if (readings[i] < min)
                min = readings[i];
        }

        return min;
    }
}
