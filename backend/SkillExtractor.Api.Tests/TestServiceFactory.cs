using Moq;
using SkillExtractor.Api.Models;
using SkillExtractor.Api.Services;

namespace SkillExtractor.Api.Tests;

/// <summary>
/// Factory for creating services with mock dependencies for testing.
/// Reduces boilerplate and ensures consistent test doubles across test files.
/// </summary>
public static class TestServiceFactory
{
    /// <summary>
    /// Creates a SkillExtractionService with a real repository and a mock matcher.
    /// Useful when testing skill extraction logic against a real database context.
    /// </summary>
    public static SkillExtractionService CreateSkillExtractionService(
        IRepository<Skill>? skillRepository = null,
        ISkillMatcher? skillMatcher = null)
    {
        skillRepository ??= new Mock<IRepository<Skill>>().Object;
        skillMatcher ??= new Mock<ISkillMatcher>().Object;

        return new SkillExtractionService(skillRepository, skillMatcher);
    }

    /// <summary>
    /// Creates a SkillExtractionService from an AppDbContext (wraps context in repository).
    /// </summary>
    public static SkillExtractionService CreateSkillExtractionServiceFromContext(
        SkillExtractor.Api.Data.AppDbContext dbContext,
        ISkillMatcher? skillMatcher = null)
    {
        var repository = new EfRepository<Skill>(dbContext);
        skillMatcher ??= new TokenBasedSkillMatcher();

        return new SkillExtractionService(repository, skillMatcher);
    }

    /// <summary>
    /// Creates a SkillsService with a real repository and a mock validator.
    /// </summary>
    public static SkillsService CreateSkillsService(
        IRepository<Skill>? skillRepository = null,
        ISkillValidator? skillValidator = null)
    {
        skillRepository ??= new Mock<IRepository<Skill>>().Object;
        skillValidator ??= new Mock<ISkillValidator>().Object;

        return new SkillsService(skillRepository, skillValidator);
    }

    /// <summary>
    /// Creates a SkillsService from an AppDbContext.
    /// </summary>
    public static SkillsService CreateSkillsServiceFromContext(
        SkillExtractor.Api.Data.AppDbContext dbContext,
        ISkillValidator? skillValidator = null)
    {
        var repository = new EfRepository<Skill>(dbContext);
        skillValidator ??= new SkillValidator(repository);

        return new SkillsService(repository, skillValidator);
    }

    /// <summary>
    /// Creates an ExtractionService with all dependencies.
    /// </summary>
    public static ExtractionService CreateExtractionService(
        SkillExtractor.Api.Data.AppDbContext dbContext,
        ISkillExtractionService? skillExtractor = null,
        IExtractionValidator? validator = null,
        IRepository<Employee>? employeeRepository = null,
        IRepository<SkillExtractor.Api.Models.CVDocument>? cvDocumentRepository = null,
        IRepository<EmployeeSkill>? employeeSkillRepository = null)
    {
        skillExtractor ??= new Mock<ISkillExtractionService>().Object;
        validator ??= new ExtractionValidator();
        employeeRepository ??= new EfRepository<Employee>(dbContext);
        cvDocumentRepository ??= new EfRepository<SkillExtractor.Api.Models.CVDocument>(dbContext);
        employeeSkillRepository ??= new EfRepository<EmployeeSkill>(dbContext);

        return new ExtractionService(dbContext, skillExtractor, validator, employeeRepository, cvDocumentRepository, employeeSkillRepository);
    }
}
