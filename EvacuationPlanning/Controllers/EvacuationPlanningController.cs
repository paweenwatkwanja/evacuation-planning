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
    public IActionResult PostEvacuationZones([FromBody] EvacuationZoneRequest request)
    {
        EvacuationZoneResponse response = _evacuationPlanningBusinessFlow.ProcessEvacuationZone(request);
        return Ok(response);
    }

    [HttpPost("vehicles")]
    public IActionResult PostVehicles([FromBody] VehicleRequest request)
    {
        VehicleResponse response = _evacuationPlanningBusinessFlow.ProcessVehicle(request);
        return Ok(response);
    }
}


