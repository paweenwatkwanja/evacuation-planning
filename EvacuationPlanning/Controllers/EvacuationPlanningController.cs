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
        LocationCoordinate locationCoordinate = new LocationCoordinate()
        {
            Latitude = 34.0522,
            Longitude = -118.2437
        };
        _context.LocationCoordinate.Add(locationCoordinate);
        _context.SaveChanges();

        EvacuationZone evacuationZone = new EvacuationZone()
        {
            ZoneID = "Z1",
            LocationCoordinateID = locationCoordinate.Id,
            NumberOfPeople = 1500,
            UrgencyLevel = 3
        };

        _context.EvacuationZone.Add(evacuationZone);
        _context.SaveChanges();

        return Ok(evacuationZone);
    }
}


