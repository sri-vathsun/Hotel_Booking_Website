using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using StayEase.Api.Data;

namespace StayEase.Api.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly ApplicationDbContext Context;
    protected readonly DbSet<T> Set;

    public GenericRepository(ApplicationDbContext context)
    {
        Context = context;
        Set = context.Set<T>();
    }

    public Task<T?> GetByIdAsync(object id, CancellationToken cancellationToken = default) => Set.FindAsync([id], cancellationToken).AsTask();
    public Task<List<T>> ListAsync(CancellationToken cancellationToken = default) => Set.AsNoTracking().ToListAsync(cancellationToken);
    public Task<List<T>> ListAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) => Set.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);
    public Task AddAsync(T entity, CancellationToken cancellationToken = default) => Set.AddAsync(entity, cancellationToken).AsTask();
    public void Update(T entity) => Set.Update(entity);
    public void Remove(T entity) => Set.Remove(entity);
    public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) => Set.AnyAsync(predicate, cancellationToken);
    public Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default) => predicate is null ? Set.CountAsync(cancellationToken) : Set.CountAsync(predicate, cancellationToken);
}
