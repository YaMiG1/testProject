namespace SkillExtractor.Api.Services;

/// <summary>
/// Generic repository abstraction (DIP: depend on this instead of DbContext directly).
/// Provides a simple, testable data access layer.
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>
    /// Gets all entities as an IQueryable for composition.
    /// </summary>
    IQueryable<T> Query();

    /// <summary>
    /// Finds an entity by primary key.
    /// </summary>
    Task<T?> GetByIdAsync(object id, CancellationToken ct = default);

    /// <summary>
    /// Adds a new entity.
    /// </summary>
    void Add(T entity);

    /// <summary>
    /// Updates an existing entity.
    /// </summary>
    void Update(T entity);

    /// <summary>
    /// Removes an entity.
    /// </summary>
    void Remove(T entity);

    /// <summary>
    /// Persists changes to the store.
    /// </summary>
    Task SaveChangesAsync(CancellationToken ct = default);
}
