using SmartX1._1.Models;

namespace SmartX1._1.Models;

public interface ITelemetryService
{
    Task StoreTelemetryAsync(TelemetryPayload payload);
    Task<IEnumerable<TelemetryRecord>> GetLatestReadingsAsync();
    Task<IEnumerable<TelemetryRecord>> GetDeviceReadingsAsync(string deviceId);
    Task<IEnumerable<TelemetryRecord>> GetReadingsByCategoryAsync(SensorCategory category);
    IReadOnlyList<TelemetryRecord> GetJaggedDeviceHistory(string deviceId);
}