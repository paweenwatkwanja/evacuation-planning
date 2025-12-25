using Models;

namespace Repository;

public class EvacuationPlanningRepository
{
    private readonly EvacuationPlanningDbContext _context;

    public EvacuationPlanningRepository(EvacuationPlanningDbContext context)
    {
        _context = context;
    }

    public void AddEvacautionZones(List<EvacuationZone> evacuationZones)
    {
        _context.EvacuationZone.AddRange(evacuationZones);
        _context.SaveChanges();
    }

    public void AddVehicles(List<Vehicle> vehicles)
    {
        _context.Vehicle.AddRange(vehicles);
        _context.SaveChanges();
    }
}