using Models;

namespace Repository;

public interface IUnitOfWork : IDisposable
{
    IRepository<EvacuationZone> EvacuationZones { get; }
    IRepository<Vehicle> Vehicles { get; }
    IRepository<EvacuationPlan> EvacuationPlans { get; }
    IRepository<EvacuationStatus> EvacuationStatuses { get; }
    IRepository<Log> Logs { get; }

    Task<int> SaveChangesAsync();
    void BeginTransaction();
    void CommitTransaction();
}