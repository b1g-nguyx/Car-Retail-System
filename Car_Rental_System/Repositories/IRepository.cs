using System.Linq;
using System.Linq.Expressions;

namespace Car_Rental_System.Repositories;

public interface IRepository<T> where T : class
{
    T? GetById(object id);
    Task<T?> GetByIdAsync(object id);

    IQueryable<T> GetQueryable(bool allowTracking = true);
    IQueryable<T> GetQueryable(Expression<Func<T, bool>> predicate, bool allowTracking = true);
    IQueryable<T> GetQueryable(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IQueryable<T>>? queryOperation = null, bool allowTracking = true);

    IEnumerable<T> GetMany(Expression<Func<T, bool>> predicate, bool allowTracking = true);
    Task<IEnumerable<T>> GetManyAsync(Expression<Func<T, bool>> predicate, bool allowTracking = true);

    IEnumerable<T> GetAll(bool allowTracking = true);
    Task<IEnumerable<T>> GetAllAsync(bool allowTracking = true);

    T? Find(object id);
    Task<T?> FindAsync(object id);
    T? Find(Expression<Func<T, bool>> predicate, bool allowTracking = true);
    Task<T?> FindAsync(Expression<Func<T, bool>> predicate, bool allowTracking = true);

    void Add(T entity);
    Task AddAsync(T entity);

    void AddRange(IEnumerable<T> entities);
    Task AddRangeAsync(IEnumerable<T> entities);

    void Update(T entity);

    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entities);

    bool Exists(object id);
    Task<bool> ExistsAsync(object id);
}
