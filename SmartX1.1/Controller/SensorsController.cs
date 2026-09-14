using Microsoft.AspNetCore.Mvc;
using SmartX.Models;
using SmartX1._1.Models;

namespace SmartX1._1.Controllers;

[ApiController]
[Route("api/sensors")]
public class SensorsController : ControllerBase
{
    private readonly ISensorProfileService _sensorService;
    private readonly ILogger<SensorsController> _logger;

    public SensorsController(ISensorProfileService sensorService, ILogger<SensorsController> logger)
    {
        _sensorService = sensorService;
        _logger = logger;
    }

    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<SensorProfile>>> GetAll()
    {
        var sensors = await _sensorService.GetAllSensorsAsync();
        return Ok(sensors);
    }

    [HttpGet("{deviceId}")]
    public async Task<ActionResult<SensorProfile>> Get(string deviceId)
    {
        var sensor = await _sensorService.GetSensorAsync(deviceId);
        if (sensor == null)
        {
            return NotFound(new { error = $"Sensor {deviceId} not found" });
        }

        return Ok(sensor);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] SensorProfile profile)
    {
        try
        {
            await _sensorService.RegisterSensorAsync(profile);
            return Ok(profile);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Sensor registration failed for {DeviceId}", profile?.DeviceId);
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error registering sensor {DeviceId}", profile?.DeviceId);
            return StatusCode(500, new { error = "An unexpected error occurred while registering the sensor." });
        }
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] SensorProfile profile)
    {
        try
        {
            await _sensorService.UpdateSensorAsync(profile);
            return Ok(profile);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error updating sensor {DeviceId}", profile?.DeviceId);
            return StatusCode(500, new { error = "An unexpected error occurred while updating the sensor." });
        }
    }
}