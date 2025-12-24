using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using Models;

[Route("/api/")]
[ApiController]
public class EvacuationPlanningController : ControllerBase
{
    private readonly EvacuationPlanningDbContext _context;

    public EvacuationPlanningController(EvacuationPlanningDbContext context)
    {
        _context = context;
    }


    [HttpPost("evacuation-zones")]
    public IActionResult PostEvacuationZones()
    {
        EvacuationZone EvacuationZone = new EvacuationZone()
        {
            Id = 1,
            ZoneID = "Z1",
            LocationCoordinate = new LocationCoordinate() { Id = 1, Latitude = 34.0522, Longitude = -118.2437 },
            NumberOfPeople = 1500,
            UrgencyLevel = 3
        };

        _context.EvacuationZone.Add(EvacuationZone);
        _context.SaveChanges();

        return Ok(EvacuationZone);
    }
}


