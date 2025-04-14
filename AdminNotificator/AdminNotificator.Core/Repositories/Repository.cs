using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AdminNotificator.Core.Repositories;

public class Repository<TEntity>(AdminNotificatorDbContext context)
    : IRepository<TEntity> where TEntity : class
{
    public IQueryable<TEntity> GetAll() => context.Set<TEntity>().AsQueryable();

    public async Task AddAsync(TEntity item, CancellationToken cancellationToken = default)
    {
        await context.Set<TEntity>().AddAsync(item, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task AddAllAsync(IEnumerable<TEntity> items, CancellationToken cancellationToken = default)
    {
        await context.Set<TEntity>().AddRangeAsync(items, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(TEntity item, CancellationToken cancellationToken = default)
    {
        context.Entry(item).State = EntityState.Modified;
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(string itemId, CancellationToken cancellationToken = default)
    {
        var entity = await context.Set<TEntity>().FindAsync(itemId);
        if (entity != null)
        {
            context.Set<TEntity>().Remove(entity);
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task DeleteAllAsync(IEnumerable<string> itemIds, CancellationToken cancellationToken = default)
    {
        var entities = new List<TEntity>();
        foreach (var id in itemIds)
        {
            var entity = await context.Set<TEntity>().FindAsync(id);
            if (entity != null)
            {
                entities.Add(entity);
            }
        }
        context.Set<TEntity>().RemoveRange(entities);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var entities = await context.Set<TEntity>().Where(predicate).ToListAsync(cancellationToken);
        context.Set<TEntity>().RemoveRange(entities);
        await context.SaveChangesAsync(cancellationToken);
    }
}