using Database;
using Models;

namespace Repository;

public class UnitOfWork : IUnitOfWork
{
    private readonly EvacuationPlanningDbContext _dbContext;

    public UnitOfWork(EvacuationPlanningDbContext dbContext)
    {
        _dbContext = dbContext;
        
        EvacuationZones = new Repository<EvacuationZone>(dbContext);
        Vehicles = new Repository<Vehicle>(dbContext);
        EvacuationPlans = new Repository<EvacuationPlan>(dbContext);
    }

    public IRepository<EvacuationZone> EvacuationZones { get; }
    public IRepository<Vehicle> Vehicles { get; }
    public IRepository<EvacuationPlan> EvacuationPlans { get; }

    public async Task<int> SaveChangesAsync()
    {
        return await _dbContext.SaveChangesAsync();
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}