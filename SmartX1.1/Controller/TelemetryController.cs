using Microsoft.AspNetCore.Mvc;
using SmartX1._1.Models;

namespace SmartX1._1.Controllers;

[ApiController]
[Route("api/telemetry")]
public class TelemetryController : ControllerBase
{
    private readonly ITelemetryService _telemetryService;
    private readonly ILogger<TelemetryController> _logger;

    public TelemetryController(ITelemetryService telemetryService, ILogger<TelemetryController> logger)
    {
        _telemetryService = telemetryService;
        _logger = logger;
    }

    [HttpGet("latest")]
    public async Task<ActionResult<IEnumerable<TelemetryRecord>>> GetLatest()
    {
        var records = await _telemetryService.GetLatestReadingsAsync();
        return Ok(records);
    }

    [HttpGet("device/{deviceId}")]
    public async Task<ActionResult<IEnumerable<TelemetryRecord>>> GetForDevice(string deviceId)
    {
        var records = await _telemetryService.GetDeviceReadingsAsync(deviceId);
        return Ok(records);
    }

    [HttpGet("device/{deviceId}/history")]
    public ActionResult<IReadOnlyList<TelemetryRecord>> GetJaggedHistory(string deviceId)
    {
        var history = _telemetryService.GetJaggedDeviceHistory(deviceId);
        return Ok(history);
    }

    [HttpPost("ingest")]
    public async Task<IActionResult> Ingest([FromBody] TelemetryPayload payload)
    {
        try
        {
            if (!LocationHierarchyValidator.IsValid(payload.DeploymentLocation))
                return BadRequest(new { error = "Deployment location must be a valid 'Child > Parent > Facility' hierarchy" });

            await _telemetryService.StoreTelemetryAsync(payload);
            return Ok(payload);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Telemetry ingestion failed for {DeviceId}", payload?.DeviceId);
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error ingesting telemetry for {DeviceId}", payload?.DeviceId);
            return StatusCode(500, new { error = "An unexpected error occurred while ingesting telemetry." });
        }
    }
}
