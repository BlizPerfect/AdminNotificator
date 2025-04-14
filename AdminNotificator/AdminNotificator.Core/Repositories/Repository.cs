using System.Linq.Expressions;
using AdminNotificator.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace AdminNotificator.Core.Repositories;

public class Repository<TEntity>(AdminNotificatorDbContext context) 
    : IRepository<TEntity> where TEntity : class
{
    public IQueryable<TEntity> GetAll() => context.Set<TEntity>().AsQueryable();

    public async Task AddAsync(string itemId, CancellationToken cancellationToken = default)
    {
        var entity = Activator.CreateInstance<TEntity>();
        if (entity is EmailType emailType)
        {
            emailType.Id = itemId;
            emailType.EmailTitle = "Default Title";
            emailType.BodyName = "Default Body";
            emailType.SenderEmail = "default@example.com";
        }
        await context.Set<TEntity>().AddAsync(entity, cancellationToken);
    }

    public async Task AddAllAsync(IEnumerable<string> itemIds, CancellationToken cancellationToken = default)
    {
        var entities = itemIds.Select(id =>
        {
            var entity = Activator.CreateInstance<TEntity>();
            if (entity is EmailType emailType)
            {
                emailType.Id = id;
                emailType.EmailTitle = "Default Title";
                emailType.BodyName = "Default Body";
                emailType.SenderEmail = "default@example.com";
            }
            return entity;
        });
        await context.Set<TEntity>().AddRangeAsync(entities, cancellationToken);
    }

    public async Task UpdateAsync(string itemId, CancellationToken cancellationToken = default)
    {
        var entity = await context.Set<TEntity>().FindAsync(itemId);
        if (entity != null)
        {
            context.Entry(entity).State = EntityState.Modified;
        }
    }

    public async Task DeleteAsync(string itemId, CancellationToken cancellationToken = default)
    {
        var entity = await context.Set<TEntity>().FindAsync(itemId);
        if (entity != null)
        {
            context.Set<TEntity>().Remove(entity);
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
    }

    public async Task DeleteAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var entities = await context.Set<TEntity>().Where(predicate).ToListAsync(cancellationToken);
        context.Set<TEntity>().RemoveRange(entities);
    }
}