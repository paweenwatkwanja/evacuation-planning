using Microsoft.AspNetCore.Mvc;
using System;

[Route("/api/evacuation-zones")]
[ApiController]
public class EvacuationPlanningController : ControllerBase
{
    [HttpPost]
    public IActionResult PostEvacuationZones()
    {
        return Ok(new EvacuationZone() { 
            ZoneID = "Z1",
            LocationCoordinates = "34.0522, -118.2437",
            NumberOfPeople = 1500,
            UrgencyLevel = 3
            });
    }
}


internal class EvacuationZone
{
    public string ZoneID { get; set; }
    public string LocationCoordinates { get; set; }
    public int NumberOfPeople { get; set; }
    public int UrgencyLevel { get; set; }
}