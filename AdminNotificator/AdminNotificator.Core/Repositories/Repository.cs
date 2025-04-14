using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace AdminNotificator.Core.Repositories;

public class Repository<TEntity>(AdminNotificatorDbContext context) 
    : IRepository<TEntity> where TEntity : class
{
    public IQueryable<TEntity> GetAll() => context.Set<TEntity>().AsQueryable();

    public async Task AddAsync(TEntity item, CancellationToken cancellationToken = default) 
        => await context.Set<TEntity>().AddAsync(item, cancellationToken);

    public async Task AddAllAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) 
        => await context.Set<TEntity>().AddRangeAsync(entities, cancellationToken);

    public Task UpdateAsync(TEntity item, CancellationToken cancellationToken = default)
    {
        context.Entry(item).State = EntityState.Modified;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(TEntity item, CancellationToken cancellationToken = default)
    {
        context.Set<TEntity>().Remove(item);
        return Task.CompletedTask;
    }

    public async Task DeleteAllAsync(IEnumerable<TEntity> items, CancellationToken cancellationToken = default) 
        => context.Set<TEntity>().RemoveRange(items);

    public async Task DeleteAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) 
        => context.Set<TEntity>().RemoveRange(await context.Set<TEntity>().Where(predicate).ToListAsync(cancellationToken));
}