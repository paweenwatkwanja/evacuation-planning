using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using Models;
using BusinessFlow;

[Route("/api/")]
[ApiController]
public class EvacuationPlanningController : ControllerBase
{
    private readonly EvacuationPlanningBusinessFlow _evacuationPlanningBusinessFlow;

    public EvacuationPlanningController(EvacuationPlanningBusinessFlow evacuationPlanningBusinessFlow)
    {
        _evacuationPlanningBusinessFlow = evacuationPlanningBusinessFlow;
    }

    [HttpPost("evacuation-zones")]
    public async Task<IActionResult> PostEvacuationZonesAsync([FromBody] List<EvacuationZoneRequest> requests)
    {
        List<EvacuationZoneResponse> responses = await _evacuationPlanningBusinessFlow.ProcessEvacuationZonesAsync(requests);
        return Ok(responses);
    }

    [HttpPost("vehicles")]
    public async Task<IActionResult> PostVehiclesAsync([FromBody] List<VehicleRequest> requests)
    {
        List<VehicleResponse> responses = await _evacuationPlanningBusinessFlow.ProcessVehiclesAsync(requests);
        return Ok(responses);
    }

    [HttpPost("evacuations/plan")]
    public IActionResult PostEvacuationPlan()
    {
        return Ok();
    }

    [HttpGet("evacuations/status")]
    public IActionResult GetEvacuationStatus()
    {
        return Ok();
    }

    [HttpPut("evacuations/update")]
    public IActionResult UpdateEvacuationStatus()
    {
        return Ok();
    }

    [HttpDelete("evacuations/clear")]
    public IActionResult DeleteEvacuations()
    {
        return Ok();
    }
}


