using Database;
using Microsoft.EntityFrameworkCore;

namespace Repository;
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly EvacuationPlanningDbContext _dbContext;
    protected readonly DbSet<T> _dbSet;

    public Repository(EvacuationPlanningDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = dbContext.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public async Task AddRangeAsync(IEnumerable<T> entities)
    {
        await _dbSet.AddRangeAsync(entities);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void DeleteRange(IEnumerable<T> entities)
    {
        _dbSet.RemoveRange(entities);
    }
}