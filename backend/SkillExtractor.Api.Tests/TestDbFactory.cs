using Microsoft.EntityFrameworkCore;
using SkillExtractor.Api.Data;

namespace SkillExtractor.Api.Tests;

/// <summary>
/// Factory for creating AppDbContext instances with different database providers for testing.
/// InMemory is used for most tests, SQLite InMemory for tests requiring transactions.
/// Each call creates a fresh, isolated database to avoid test interference.
/// </summary>
public static class TestDbFactory
{
    /// <summary>
    /// Creates a new AppDbContext with InMemory database using a unique name.
    /// </summary>
    /// <param name="databaseName">Optional database name. If not provided, a unique GUID-based name is generated.</param>
    /// <returns>A new AppDbContext instance with InMemory provider.</returns>
    public static AppDbContext CreateDbContext(string? databaseName = null)
    {
        databaseName ??= Guid.NewGuid().ToString();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;

        var context = new AppDbContext(options);
        context.Database.EnsureCreated();

        return context;
    }

    /// <summary>
    /// Creates and seeds a database context with test data.
    /// </summary>
    /// <param name="seedAction">Action to populate the context with test data.</param>
    /// <param name="databaseName">Optional database name for the test context.</param>
    /// <returns>A seeded AppDbContext instance.</returns>
    public static AppDbContext CreateDbContextWithSeed(
        Action<AppDbContext> seedAction,
        string? databaseName = null)
    {
        var context = CreateDbContext(databaseName);
        seedAction(context);
        context.SaveChanges();
        return context;
    }

    /// <summary>
    /// Creates a new AppDbContext with SQLite InMemory database.
    /// Use this for tests that require transaction support (InMemory doesn't support transactions).
    /// </summary>
    /// <returns>A new AppDbContext instance with SQLite InMemory provider.</returns>
    public static AppDbContext CreateSqliteDbContext()
    {
        var connectionString = $"Data Source=file:{Guid.NewGuid()}?mode=memory&cache=shared";

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connectionString)
            .Options;

        var context = new AppDbContext(options);
        context.Database.EnsureCreated();

        return context;
    }

    /// <summary>
    /// Creates and seeds a SQLite InMemory database context with test data.
    /// Use this for tests that require transaction support.
    /// </summary>
    /// <param name="seedAction">Action to populate the context with test data.</param>
    /// <returns>A seeded AppDbContext instance with SQLite provider.</returns>
    public static AppDbContext CreateSqliteDbContextWithSeed(Action<AppDbContext> seedAction)
    {
        var context = CreateSqliteDbContext();
        seedAction(context);
        context.SaveChanges();
        return context;
    }
}
