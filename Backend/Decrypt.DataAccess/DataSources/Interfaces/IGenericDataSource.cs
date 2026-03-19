using System.Linq.Expressions;
using Decrypt.DataAccess.Entities;


namespace Decrypt.DataAccess.DataSources.Interfaces;

public interface IGenericDataSource<T> where T : BaseEntity
{
    /// <summary>
    /// Get no-tracking items according to the given filter expression
    /// </summary>
    /// <param name="filter">Expression to filter items</param>
    /// <returns>Read-only collection of items</returns>
    Task<IReadOnlyList<T>> GetItemsAsync(Expression<Func<T, bool>> filter);

    /// <summary>
    /// Get queryable collection of not tracking entities
    /// </summary>
    IQueryable<T> GetQueryableItems();

    /// <summary>
    /// Get all no-tracking items 
    /// </summary>
    /// <returns></returns>
    Task<IReadOnlyList<T>> GetAllAsync();

    /// <summary>
    /// Get the entity with given id if exists
    /// </summary>
    /// <param name="id">Id to get the entity</param>
    Task<T?> GetByIdAsync(int id);

    /// <summary>
    /// Add the entity to DB
    /// </summary>
    /// <param name="item">Entity to add</param>
    Task<T> AddAsync(T item);

    /// <summary>
    /// Update the entity
    /// </summary>
    /// <param name="item">updated item to vbe saved</param>
    Task UpdateAsync(T item);

    /// <summary>
    /// Remove the entity
    /// </summary>
    /// <param name="item">Entity to delete</param>
    Task DeleteAsync(T item);

    /// <summary>
    /// Add the range of entities 
    /// </summary>
    /// <param name="itemRange">The range to add</param>
    Task AddRangeAsync(T[] itemRange);
}