using System.Linq.Expressions;
using Car_Rental_System.Data;
using Microsoft.EntityFrameworkCore;

namespace Car_Rental_System.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly ApplicationDbContext _dbContext;
    protected readonly DbSet<T> _entities;

    public Repository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        _entities = _dbContext.Set<T>();
    }

    public T? GetById(object id)
    {
        return _entities.Find(id);
    }

    public async Task<T?> GetByIdAsync(object id)
    {
        return await _entities.FindAsync(id);
    }


    public IQueryable<T> GetQueryable(bool allowTracking = true)
    {
        var query = _entities.AsQueryable();
        query = ApplyTracking(query, allowTracking);
        return query;
    }

    public IEnumerable<T> GetMany(Expression<Func<T, bool>> predicate, bool allowTracking = true)
    {
        var query = _entities.AsQueryable();
        query = ApplyTracking(query, allowTracking);
        return query.Where(predicate);
    }

    public async Task<IEnumerable<T>> GetManyAsync(Expression<Func<T, bool>> predicate, bool allowTracking = true)
    {
        var query = _entities.AsQueryable();
        query = ApplyTracking(query, allowTracking);
        return await query.Where(predicate).ToListAsync();
    }

    public T? Find(object id)
    {
        return _entities.Find(id);
    }

    public async Task<T?> FindAsync(object id)
    {
        return await _entities.FindAsync(id);
    }

    public T? Find(Expression<Func<T, bool>> predicate, bool allowTracking = true)
    {
        var query = _entities.AsQueryable();
        query = ApplyTracking(query, allowTracking);
        return query.FirstOrDefault(predicate);
    }

    public async Task<T?> FindAsync(Expression<Func<T, bool>> predicate, bool allowTracking = true)
    {
        var query = _entities.AsQueryable();
        query = ApplyTracking(query, allowTracking);
        return await query.FirstOrDefaultAsync(predicate);
    }

    public async Task<T?> FindAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>>[]? queryOperation = null)
    {
        var query = _entities.AsQueryable();

        if (queryOperation != null)
        {
            foreach (var includeProperty in queryOperation)
            {
                query = query.Include(includeProperty);
            }
        }

        return await query.FirstOrDefaultAsync(predicate);
    }

    public async Task<T?> FindAsync(Expression<Func<T, bool>> predicate)
    {
        var query = _entities.AsQueryable();
        return await query.FirstOrDefaultAsync(predicate);
    }

    public void Add(T entity)
    {
        _entities.Add(entity);
    }

    public async Task AddAsync(T entity)
    {
        await _entities.AddAsync(entity);
    }

    public void AddRange(IEnumerable<T> entities)
    {
        _entities.AddRange(entities);
    }

    public async Task AddRangeAsync(IEnumerable<T> entities)
    {
        await _entities.AddRangeAsync(entities);
    }

    public void Update(T entity)
    {
        _entities.Update(entity);
    }

    public void Remove(T entity)
    {
        _entities.Remove(entity);
    }

    public void RemoveRange(IEnumerable<T> entities)
    {
        _entities.RemoveRange(entities);
    }

    private IQueryable<T> ApplyTracking(IQueryable<T> query, bool allowTracking)
    {
        return allowTracking ? query : query.AsNoTracking();
    }

    public IQueryable<T> GetQueryableFromSqlQuery(string sql, bool allowTracking = true)
    {
        var query = _entities.FromSqlRaw(sql);
        query = ApplyTracking(query, allowTracking);
        return query.AsQueryable();
    }

    public IQueryable<T> GetQueryable(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IQueryable<T>>? queryOperation = null, bool allowTracking = true)
    {
        var query = _entities.Where(predicate);
        query = ApplyTracking(query, allowTracking);

        if (queryOperation != null)
        {
            query = queryOperation(query);
        }

        return query.AsQueryable();
    }

    public IQueryable<T> GetQueryable(Expression<Func<T, bool>> predicate, bool allowTracking = true)
    {
        var query = _entities.Where(predicate);
        query = ApplyTracking(query, allowTracking);

        return query.AsQueryable();
    }

    public bool Exists(object id)
    {
        return _entities.Find(id) != null;
    }

    public async Task<bool> ExistsAsync(object id)
    {
        var entity = await _entities.FindAsync(id);
        return entity != null;
    }

    public IEnumerable<T> GetAll(bool allowTracking = true)
    {
        var query = _entities.AsQueryable();
        query = ApplyTracking(query, allowTracking);
        return query.AsEnumerable();
    }

    public async Task<IEnumerable<T>> GetAllAsync(bool allowTracking = true)
    {
        var query = _entities.AsQueryable();
        query = ApplyTracking(query, allowTracking);
        return await query.ToListAsync();
    }
}
