using SmartX.Models;

namespace SmartX1._1.Models;

public interface ISensorProfileService
{
    Task RegisterSensorAsync(SensorProfile profile);
    Task<SensorProfile?> GetSensorAsync(string deviceId);
    Task<IEnumerable<SensorProfile>> GetAllSensorsAsync();
    Task UpdateSensorAsync(SensorProfile profile);
}