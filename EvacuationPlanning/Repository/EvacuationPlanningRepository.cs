using Models;

namespace Repository;

public class EvacuationPlanningRepository
{
    private readonly EvacuationPlanningDbContext _context;

    public EvacuationPlanningRepository(EvacuationPlanningDbContext context)
    {
        _context = context;
    }

    public void AddEvacautionZone(EvacuationZone evacuationZone)
    {
        _context.EvacuationZone.Add(evacuationZone);
        _context.SaveChanges();
    }

    public void AddVehicle(Vehicle vehicle)
    {
        _context.Vehicle.Add(vehicle);
        _context.SaveChanges();
    }
}