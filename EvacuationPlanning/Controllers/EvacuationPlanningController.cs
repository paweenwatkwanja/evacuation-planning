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
    public IActionResult PostEvacuationZones([FromBody] List<EvacuationZoneRequest> requests)
    {
        List<EvacuationZoneResponse> responses = _evacuationPlanningBusinessFlow.ProcessEvacuationZones(requests);
        return Ok(responses);
    }

    [HttpPost("vehicles")]
    public IActionResult PostVehicles([FromBody] List<VehicleRequest> requests)
    {
        List<VehicleResponse> responses = _evacuationPlanningBusinessFlow.ProcessVehicles(requests);
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


