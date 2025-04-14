using System.Linq.Expressions;

namespace AdminNotificator.Core.Repositories;

public interface IRepository<TEntity> where TEntity : class
{
    /// <summary>
    ///     Получение всех сущностей их хранилища
    /// </summary>
    /// <returns>Все сущности в хранилище</returns>
    IQueryable<TEntity> GetAll();

    /// <summary>
    ///     Добавление сущности в хранилище
    /// </summary>
    /// <param name="item">Сущность</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Сущность после добавления</returns>
    Task AddAsync(TEntity item, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Добавление указанных сущностей
    /// </summary>
    /// <param name="items">Сущности</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task AddAllAsync(IEnumerable<TEntity> items, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Обновление сущности
    /// </summary>
    /// <param name="item">Сущность</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task UpdateAsync(TEntity item, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Удаление сущности
    /// </summary>
    /// <param name="itemId">Идентификатор сущности</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task DeleteAsync(string itemId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Удаление указанных сущностей
    /// </summary>
    /// <param name="itemIds">Идентификаторы сущностей</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task DeleteAllAsync(IEnumerable<string> itemIds, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Удаление сущностей по условию
    /// </summary>
    /// <param name="predicate">Условие выборки сущностей для удаления</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task DeleteAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
}
