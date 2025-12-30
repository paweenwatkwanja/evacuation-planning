using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using Models;
using BusinessFlow;
using System.Threading.Tasks;
using System.Collections.Generic;

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
        return Created("evacuation-zones", responses);
    }

    [HttpPost("vehicles")]
    public async Task<IActionResult> PostVehiclesAsync([FromBody] List<VehicleRequest> requests)
    {
        List<VehicleResponse> responses = await _evacuationPlanningBusinessFlow.ProcessVehiclesAsync(requests);
        return Created("vehicles", responses);
    }

    [HttpPost("evacuations/plan")]
    public async Task<IActionResult> PostEvacuationPlanAsync()
    {
        List<EvacuationPlan> responses = await _evacuationPlanningBusinessFlow.ProcessEvacuationPlanAsync();
        return Ok(responses);
    }

    [HttpGet("evacuations/status")]
    public async Task<IActionResult> GetEvacuationStatusAsync()
    {
        List<EvacuationStatus> responses = await _evacuationPlanningBusinessFlow.GetEvacuationStatusesAsync();
        return Ok(responses);
    }

    [HttpPut("evacuations/update/{id}")]
    public async Task<IActionResult> UpdateEvacuationStatusAsync(int id, [FromBody] EvacuationStatusUpdateRequest request)
    {
        await _evacuationPlanningBusinessFlow.UpdateEvacuationStatusAndRelatedDataAsync(id, request);
        return Ok();
    }

    [HttpDelete("evacuations/clear")]
    public async Task<IActionResult> DeleteEvacuationsAsync()
    {
        await _evacuationPlanningBusinessFlow.DeleteAllDataAsync();
        return Ok();
    }
}


