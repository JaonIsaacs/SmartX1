namespace SmartX1._1.Models;

public class TelemetryService : ITelemetryService
{
    private readonly List<TelemetryRecord> _telemetryData = new();
    private readonly TelemetryHistoryStore _historyStore = new();
    private readonly object _lock = new object();

    public Task StoreTelemetryAsync(TelemetryPayload payload)
    {
        if (string.IsNullOrWhiteSpace(payload.DeviceId))
            throw new ArgumentException("DeviceId is required");

        if (string.IsNullOrWhiteSpace(payload.Value))
            throw new ArgumentException("Value is required");

        var record = new TelemetryRecord
        {
            DeviceId = payload.DeviceId,
            Category = payload.Category,
            DeploymentLocation = payload.DeploymentLocation,
            Value = payload.Value,
            Unit = payload.Unit,
            CollectionTime = payload.CollectionTime ?? DateTime.UtcNow,
            RecordedAt = DateTime.UtcNow
        };

        lock (_lock)
        {
            _telemetryData.Add(record);
        }

        // Track sequential per-device history in a jagged array structure.
        _historyStore.Append(record);

        return Task.CompletedTask;
    }

    public Task<IEnumerable<TelemetryRecord>> GetLatestReadingsAsync()
    {
        lock (_lock)
        {
            var latest = _telemetryData
                .GroupBy(r => r.DeviceId)
                .Select(g => g.OrderByDescending(r => r.CollectionTime).First())
                .ToList();

            return Task.FromResult(latest.AsEnumerable());
        }
    }

    public Task<IEnumerable<TelemetryRecord>> GetDeviceReadingsAsync(string deviceId)
    {
        lock (_lock)
        {
            var readings = _telemetryData
                .Where(r => r.DeviceId == deviceId)
                .OrderByDescending(r => r.CollectionTime)
                .Take(100)
                .ToList();

            return Task.FromResult(readings.AsEnumerable());
        }
    }

    public Task<IEnumerable<TelemetryRecord>> GetReadingsByCategoryAsync(SensorCategory category)
    {
        lock (_lock)
        {
            var readings = _telemetryData
                .Where(r => r.Category == category)
                .OrderByDescending(r => r.CollectionTime)
                .Take(100)
                .ToList();

            return Task.FromResult(readings.AsEnumerable());
        }
    }

    /// <summary>
    /// Returns the full sequential jagged-array history for one device
    /// </summary>
    public IReadOnlyList<TelemetryRecord> GetJaggedDeviceHistory(string deviceId)
    {
        return _historyStore.GetDeviceHistory(deviceId);
    }
}