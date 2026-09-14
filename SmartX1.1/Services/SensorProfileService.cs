using LiteDB;
using SmartX.Models;
using SmartX1._1.Models;

namespace SmartX1._1.Services;

public class SensorProfileService : ISensorProfileService
{
    private const string CollectionName = "sensors";
    private readonly ILiteDatabase _database;

    public SensorProfileService(ILiteDatabase database)
    {
        _database = database;

        var collection = _database.GetCollection<SensorProfile>(CollectionName);
        collection.EnsureIndex(s => s.DeviceId, unique: true);
    }

    public Task RegisterSensorAsync(SensorProfile profile)
    {
        if (string.IsNullOrWhiteSpace(profile.DeviceId))
            throw new ArgumentException("DeviceId is required");

        var collection = _database.GetCollection<SensorProfile>(CollectionName);

        if (collection.Exists(s => s.DeviceId == profile.DeviceId))
            throw new ArgumentException($"Device {profile.DeviceId} is already registered");

        profile.RegistrationDate = DateTime.UtcNow;
        collection.Insert(profile);

        return Task.CompletedTask;
    }

    public Task<SensorProfile?> GetSensorAsync(string deviceId)
    {
        var collection = _database.GetCollection<SensorProfile>(CollectionName);
        var result = collection.FindOne(s => s.DeviceId == deviceId);

        return Task.FromResult<SensorProfile?>(result);
    }

    public Task<IEnumerable<SensorProfile>> GetAllSensorsAsync()
    {
        var collection = _database.GetCollection<SensorProfile>(CollectionName);
        var results = collection.FindAll().ToList();

        return Task.FromResult<IEnumerable<SensorProfile>>(results);
    }

    public Task UpdateSensorAsync(SensorProfile profile)
    {
        var collection = _database.GetCollection<SensorProfile>(CollectionName);
        var existing = collection.FindOne(s => s.Id == profile.Id);

        if (existing == null)
            throw new ArgumentException("Sensor not found");

        existing.Name = profile.Name;
        existing.Category = profile.Category;
        existing.DeploymentLocation = profile.DeploymentLocation;
        existing.Unit = profile.Unit;
        existing.IsActive = profile.IsActive;

        collection.Update(existing);

        return Task.CompletedTask;
    }
}