using Microsoft.AspNetCore.Mvc;
using SmartX1._1.Models;

namespace SmartX1._1.Controllers;

[ApiController]
[Route("api/aggregation")]
public class AggregationController : ControllerBase
{
    private readonly ITelemetryService _telemetryService;
    private readonly TelemetryAggregation _aggregation = new();

    public AggregationController(ITelemetryService telemetryService)
    {
        _telemetryService = telemetryService;
    }

    /// <summary>
    /// Sums the latest reading from every sensor in a given category using the overloaded '+' operator on <see cref="TelemetryPacket{T}"/>.
    /// </summary>
    [HttpGet("zone-total/{category}")]
    public async Task<IActionResult> GetZoneTotal(SensorCategory category)
    {
        var readings = await _telemetryService.GetReadingsByCategoryAsync(category);

        var packets = readings
            .Where(r => float.TryParse(r.Value, out _))
            .Select(r => new TelemetryPacket<float>(
                float.Parse(r.Value), r.DeviceId, r.Category, r.DeploymentLocation, r.Unit))
            .ToArray();

        if (packets.Length == 0)
            return Ok(new { total = 0, unit = "", deviceCount = 0 });

        var total = _aggregation.AggregatePowerMeters(packets);
        var max = _aggregation.FindMaximum(packets);

        return Ok(new
        {
            total = total.Value,
            unit = total.Unit,
            deviceCount = packets.Length,
            peakDevice = max.DeviceId,
            peakValue = max.Value
        });
    }
}