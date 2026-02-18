using Microsoft.EntityFrameworkCore;
using SkillExtractor.Api.Data;

namespace SkillExtractor.Api.Services;

/// <summary>
/// Generic repository implementation wrapping AppDbContext (DIP: abstraction over concrete DbContext).
/// Reduces coupling between services and EF Core.
/// </summary>
public class EfRepository<T> : IRepository<T> where T : class
{
    private readonly AppDbContext _context;

    public EfRepository(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public IQueryable<T> Query() => _context.Set<T>();

    public async Task<T?> GetByIdAsync(object id, CancellationToken ct = default)
    {
        return await _context.Set<T>().FindAsync(new object[] { id }, cancellationToken: ct);
    }

    public void Add(T entity) => _context.Set<T>().Add(entity);

    public void Update(T entity) => _context.Set<T>().Update(entity);

    public void Remove(T entity) => _context.Set<T>().Remove(entity);

    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await _context.SaveChangesAsync(ct);
    }
}
