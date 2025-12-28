using Microsoft.EntityFrameworkCore;
using Models;

namespace Database;

public class EvacuationPlanningDbContext : DbContext
{
    public EvacuationPlanningDbContext(DbContextOptions<EvacuationPlanningDbContext> options)
        : base(options)
    {
    }

    public DbSet<EvacuationZone> EvacuationZone { get; set; }
    public DbSet<Vehicle> Vehicle { get; set; }
    public DbSet<EvacuationPlan> EvacuationPlan { get; set; }
    public DbSet<Log> Log { get; set; }
}