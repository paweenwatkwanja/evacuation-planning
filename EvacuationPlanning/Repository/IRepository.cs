using System.Linq.Expressions;

namespace Repository;

public interface IRepository<T> where T : class
{
    Task<List<T>> GetAllAsync(params string[] include);
    Task<List<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null);
    Task AddAsync(T entity);
    Task AddRangeAsync(List<T> entities);
    void Update(T entity);
    void DeleteRange(List<T> entities);
}