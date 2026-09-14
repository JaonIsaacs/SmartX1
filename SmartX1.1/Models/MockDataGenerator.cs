using SmartX.Models;
using SmartX1._1.Models;

namespace SmartX.Services;

/// <summary>
/// Generates realistic mock telemetry data for testing without physical IoT devices.
/// </summary>
public class MockDataGenerator
{
    private readonly Random _random = new Random();

    /// <summary>
    /// Generates a mock environmental telemetry packet (temperature).
    /// </summary>
    public TelemetryPayload GenerateTemperatureReading(string deviceId, string location)
    {
        var baseTemp = 22.0f; // Room temperature baseline
        var variation = (float)(_random.NextDouble() - 0.5) * 8; // ±4°C variation
        var temp = baseTemp + variation;

        return new TelemetryPayload
        {
            DeviceId = deviceId,
            Value = temp.ToString("F2"),
            Category = SensorCategory.Environmental,
            DeploymentLocation = location,
            Unit = "°C",
            CollectionTime = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Generates a mock power consumption reading.
    /// </summary>
    public TelemetryPayload GeneratePowerReading(string deviceId, string location)
    {
        var baseLoad = 150; // Base power in Watts
        var variance = _random.Next(-50, 100); // ±50W variance
        var watts = baseLoad + variance;

        return new TelemetryPayload
        {
            DeviceId = deviceId,
            Value = Math.Max(0, watts).ToString(),
            Category = SensorCategory.PowerConsumption,
            DeploymentLocation = location,
            Unit = "W",
            CollectionTime = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Generates a mock actuator/switch state reading.
    /// </summary>
    public TelemetryPayload GenerateSwitchReading(string deviceId, string location)
    {
        var state = _random.Next(2) == 0 ? "0" : "1"; // 0 = OFF, 1 = ON

        return new TelemetryPayload
        {
            DeviceId = deviceId,
            Value = state,
            Category = SensorCategory.Actuator,
            DeploymentLocation = location,
            Unit = "state",
            CollectionTime = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Gets a collection of pre-configured test devices.
    /// </summary>
    public List<SensorProfile> GetTestSensorProfiles()
    {
        return new List<SensorProfile>
        {
            new SensorProfile
            {
                DeviceId = "ESP32:001A2B3C4D5E",
                Name = "Farm A - Temperature Sensor",
                Category = SensorCategory.Environmental,
                DeploymentLocation = "Sub-Zone A > Zone 1 > Hydroponic Farm",
                Unit = "°C",
                DataType = "float",
                IsActive = true,
                ConfigurationJson = "{\"pin\": 32, \"interval\": 5000}"
            },
            new SensorProfile
            {
                DeviceId = "ESP32:11223344FF55",
                Name = "Farm A - Power Meter",
                Category = SensorCategory.PowerConsumption,
                DeploymentLocation = "Sub-Zone A > Zone 1 > Hydroponic Farm",
                Unit = "W",
                DataType = "int",
                IsActive = true,
                ConfigurationJson = "{\"pin\": 33, \"interval\": 10000}"
            },
            new SensorProfile
            {
                DeviceId = "ESP32:AA11BB22CC33",
                Name = "Irrigation Control - Zone 1",
                Category = SensorCategory.Actuator,
                DeploymentLocation = "Sub-Zone A > Zone 1 > Valve Control",
                Unit = "state",
                DataType = "bool",
                IsActive = true,
                ConfigurationJson = "{\"pin\": 25, \"type\": \"relay\"}"
            },
            new SensorProfile
            {
                DeviceId = "ESP32:DDC0FFEE0000",
                Name = "Real Estate - Temperature",
                Category = SensorCategory.Environmental,
                DeploymentLocation = "Sub-Zone B > Zone 2 > Property Unit 5",
                Unit = "°C",
                DataType = "float",
                IsActive = true,
                ConfigurationJson = "{\"pin\": 34, \"interval\": 5000}"
            },
            new SensorProfile
            {
                DeviceId = "ESP32:DEADBEEF1234",
                Name = "Smart Grid - Power Usage",
                Category = SensorCategory.PowerConsumption,
                DeploymentLocation = "Sub-Zone B > Zone 2 > Smart Meter",
                Unit = "W",
                DataType = "int",
                IsActive = true,
                ConfigurationJson = "{\"pin\": 35, \"interval\": 15000}"
            },
            new SensorProfile
            {
                DeviceId = "ESP32:CAFEBABE5678",
                Name = "Utility Tracker - Smart Switch",
                Category = SensorCategory.Actuator,
                DeploymentLocation = "Sub-Zone B > Zone 2 > Distribution Panel",
                Unit = "state",
                DataType = "bool",
                IsActive = true,
                ConfigurationJson = "{\"pin\": 26, \"type\": \"relay\"}"
            }
        };
    }
}