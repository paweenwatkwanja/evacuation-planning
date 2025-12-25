using Models;

namespace Repository;

public interface IUnitOfWork : IDisposable
{
    IRepository<EvacuationZone> EvacuationZones { get; }
    IRepository<Vehicle> Vehicles { get; }
    
    Task<int> SaveChangesAsync();
}