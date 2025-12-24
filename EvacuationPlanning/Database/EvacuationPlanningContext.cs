using Microsoft.EntityFrameworkCore;

namespace Models;

public class EvacuationPlanningDbContext : DbContext
{
    public EvacuationPlanningDbContext(DbContextOptions<EvacuationPlanningDbContext> options)
        : base(options)
    {
    }

    public DbSet<EvacuationZone> EvacuationZone { get; set; }
    public DbSet<LocationCoordinate> LocationCoordinate { get; set; }
}