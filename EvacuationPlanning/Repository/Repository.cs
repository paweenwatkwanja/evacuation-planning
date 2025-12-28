using Database;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

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

    public async Task<List<T>> GetAllAsync(params string[] includes)
    {
        IQueryable<T> query = _dbSet;

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return await query.ToListAsync();
    }

    public async Task<List<T>> FindAsync(
         Expression<Func<T, bool>> predicate,
         Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null)
    {
        var query = _dbSet.Where(predicate);

        if (orderBy != null)
        {
            query = orderBy(query);
        }

        return await query.ToListAsync();
    }

    public async Task<T?> FindOneAsync(
    Expression<Func<T, bool>> predicate,
    Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null)
    {
        var query = _dbSet.Where(predicate);

        if (orderBy != null)
        {
            query = orderBy(query);
        }

        return await query.FirstOrDefaultAsync();
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public async Task AddRangeAsync(List<T> entities)
    {
        await _dbSet.AddRangeAsync(entities);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public async Task DeleteAllAsync()
    {
        await _dbSet.ExecuteDeleteAsync();
    }
}