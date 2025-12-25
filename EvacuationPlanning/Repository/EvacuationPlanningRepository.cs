using Models;

namespace Repository;

public class EvacuationPlanningRepository
{
     private readonly EvacuationPlanningDbContext _context;

    public EvacuationPlanningRepository(EvacuationPlanningDbContext context)
    {
        _context = context;
    }


    public void AddLocationCoordinate(LocationCoordinate locationCoordinate)
    {
        _context.LocationCoordinate.Add(locationCoordinate);
        _context.SaveChanges();
    }

    public void AddEvacautionZone(EvacuationZone evacuationZone)
    {
        _context.EvacuationZone.Add(evacuationZone);
        _context.SaveChanges();
    }
}