using FluentAssertions;
using Moq;
using SkillExtractor.Api.DTOs;
using SkillExtractor.Api.Models;
using SkillExtractor.Api.Services;
using Xunit;

namespace SkillExtractor.Api.Tests;

/// <summary>
/// Tests for ExtractionService. Uses SQLite InMemory database because
/// ExtractionService requires transaction support (EF Core InMemory doesn't support transactions).
/// </summary>
public class ExtractionServiceTests
{
    [Fact]
    public async Task ExtractAndSaveAsync_WithValidInput_CreatesEmployeeAndCVDocument()
    {
        // Arrange
        var context = TestDbFactory.CreateSqliteDbContextWithSeed(ctx =>
        {
            ctx.Skills.Add(new Skill { Name = "C#", Aliases = null });
            ctx.Skills.Add(new Skill { Name = ".NET", Aliases = null });
        });

        var mockSkillExtractor = new Mock<ISkillExtractionService>();
        var skills = new List<Skill> 
        { 
            new Skill { Id = 1, Name = "C#", Aliases = null },
            new Skill { Id = 2, Name = ".NET", Aliases = null }
        };
        mockSkillExtractor
            .Setup(x => x.ExtractSkillsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(skills);

        var service = new ExtractionService(context, mockSkillExtractor.Object);
        var dto = new ExtractRequestDto 
        { 
            FullName = "John Doe",
            Email = "john@example.com",
            RawText = "C# .NET"
        };

        // Act
        var (ok, result, error) = await service.ExtractAndSaveAsync(dto, CancellationToken.None);

        // Assert
        ok.Should().BeTrue($"Expected extraction to succeed but got error: {error}");
        error.Should().BeNull();
        result.Should().NotBeNull();
        result!.EmployeeId.Should().BeGreaterThan(0);

        // Verify employee was created
        var employee = context.Employees.First(e => e.Id == result.EmployeeId);
        employee.FullName.Should().Be("John Doe");
        employee.Email.Should().Be("john@example.com");

        // Verify CV document was created
        var cvDoc = context.CVDocuments.First(c => c.EmployeeId == result.EmployeeId);
        cvDoc.RawText.Should().Be("C# .NET");

        // Verify employee skills were created
        var employeeSkills = context.EmployeeSkills.Where(es => es.EmployeeId == result.EmployeeId).ToList();
        employeeSkills.Should().HaveCount(2);
    }

    [Fact]
    public async Task ExtractAndSaveAsync_ExtractedSkillsReturned_InResponse()
    {
        // Arrange
        var context = TestDbFactory.CreateSqliteDbContextWithSeed(ctx =>
        {
            ctx.Skills.Add(new Skill { Name = "C#", Aliases = null });
            ctx.Skills.Add(new Skill { Name = ".NET", Aliases = null });
        });

        var mockSkillExtractor = new Mock<ISkillExtractionService>();
        var skills = new List<Skill>
        {
            new Skill { Id = 1, Name = "C#", Aliases = null },
            new Skill { Id = 2, Name = ".NET", Aliases = null }
        };
        mockSkillExtractor
            .Setup(x => x.ExtractSkillsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(skills);

        var service = new ExtractionService(context, mockSkillExtractor.Object);
        var dto = new ExtractRequestDto
        {
            FullName = "Jane Doe",
            Email = "jane@example.com",
            RawText = "C# .NET"
        };

        // Act
        var (ok, result, error) = await service.ExtractAndSaveAsync(dto, CancellationToken.None);

        // Assert
        ok.Should().BeTrue($"Expected extraction to succeed but got error: {error}");
        result?.ExtractedSkills.Should().HaveCount(2);
        result?.ExtractedSkills.Select(s => s.Name).Should().Contain(new[] { "C#", ".NET" });
    }

    [Fact]
    public async Task ExtractAndSaveAsync_WithNoDuplicateSkills_CreatesSingleEmployeeSkillPerSkill()
    {
        // Arrange
        var context = TestDbFactory.CreateSqliteDbContextWithSeed(ctx =>
        {
            ctx.Skills.Add(new Skill { Name = "C#", Aliases = null });
        });

        var mockSkillExtractor = new Mock<ISkillExtractionService>();
        // Skill mentioned multiple times in text, but should create only one EmployeeSkill entry
        var skills = new List<Skill> { new Skill { Id = 1, Name = "C#", Aliases = null } };
        mockSkillExtractor
            .Setup(x => x.ExtractSkillsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(skills);

        var service = new ExtractionService(context, mockSkillExtractor.Object);
        var dto = new ExtractRequestDto
        {
            FullName = "Bob Smith",
            Email = null,
            RawText = "C# C# C#"
        };

        // Act
        var (ok, result, error) = await service.ExtractAndSaveAsync(dto, CancellationToken.None);

        // Assert
        ok.Should().BeTrue($"Expected extraction to succeed but got error: {error}");
        var employeeSkills = context.EmployeeSkills.Where(es => es.EmployeeId == result!.EmployeeId).ToList();
        employeeSkills.Should().HaveCount(1);
        employeeSkills.First().SkillId.Should().Be(1);
    }

    [Fact]
    public async Task ExtractAndSaveAsync_WithoutEmail_CreatesEmployeeSuccessfully()
    {
        // Arrange
        var context = TestDbFactory.CreateSqliteDbContext();

        var mockSkillExtractor = new Mock<ISkillExtractionService>();
        mockSkillExtractor
            .Setup(x => x.ExtractSkillsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Skill>());

        var service = new ExtractionService(context, mockSkillExtractor.Object);
        var dto = new ExtractRequestDto
        {
            FullName = "Alice",
            Email = null,
            RawText = "Some CV text"
        };

        // Act
        var (ok, result, error) = await service.ExtractAndSaveAsync(dto, CancellationToken.None);

        // Assert
        ok.Should().BeTrue($"Expected extraction to succeed but got error: {error}");
        var employee = context.Employees.First(e => e.Id == result!.EmployeeId);
        employee.Email.Should().BeNull();
    }

    [Fact]
    public async Task ExtractAndSaveAsync_TrimsWhitespace_FromFullNameAndEmail()
    {
        // Arrange
        var context = TestDbFactory.CreateSqliteDbContext();

        var mockSkillExtractor = new Mock<ISkillExtractionService>();
        mockSkillExtractor
            .Setup(x => x.ExtractSkillsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Skill>());

        var service = new ExtractionService(context, mockSkillExtractor.Object);
        var dto = new ExtractRequestDto
        {
            FullName = "  John Doe  ",
            Email = "  john@example.com  ",
            RawText = "CV text"
        };

        // Act
        var (ok, result, error) = await service.ExtractAndSaveAsync(dto, CancellationToken.None);

        // Assert
        ok.Should().BeTrue($"Expected extraction to succeed but got error: {error}");
        var employee = context.Employees.First(e => e.Id == result!.EmployeeId);
        employee.FullName.Should().Be("John Doe");
        employee.Email.Should().Be("john@example.com");
    }

    [Fact]
    public async Task ExtractAndSaveAsync_WithNullFullName_ReturnsValidationError()
    {
        // Arrange
        var context = TestDbFactory.CreateSqliteDbContext();
        var mockSkillExtractor = new Mock<ISkillExtractionService>();

        var service = new ExtractionService(context, mockSkillExtractor.Object);
        var dto = new ExtractRequestDto
        {
            FullName = null!,
            Email = "test@example.com",
            RawText = "CV text"
        };

        // Act
        var (ok, result, error) = await service.ExtractAndSaveAsync(dto, CancellationToken.None);

        // Assert
        ok.Should().BeFalse();
        error.Should().Be("validation");
        result.Should().BeNull();
    }

    [Fact]
    public async Task ExtractAndSaveAsync_WithEmptyRawText_ReturnsValidationError()
    {
        // Arrange
        var context = TestDbFactory.CreateSqliteDbContext();
        var mockSkillExtractor = new Mock<ISkillExtractionService>();

        var service = new ExtractionService(context, mockSkillExtractor.Object);
        var dto = new ExtractRequestDto
        {
            FullName = "John Doe",
            Email = "john@example.com",
            RawText = "   "
        };

        // Act
        var (ok, result, error) = await service.ExtractAndSaveAsync(dto, CancellationToken.None);

        // Assert
        ok.Should().BeFalse();
        error.Should().Be("validation");
        result.Should().BeNull();
    }

    [Fact]
    public async Task ExtractAndSaveAsync_WithNullRequest_ReturnsValidationError()
    {
        // Arrange
        var context = TestDbFactory.CreateSqliteDbContext();
        var mockSkillExtractor = new Mock<ISkillExtractionService>();

        var service = new ExtractionService(context, mockSkillExtractor.Object);

        // Act
        var (ok, result, error) = await service.ExtractAndSaveAsync(null!, CancellationToken.None);

        // Assert
        ok.Should().BeFalse();
        error.Should().Be("validation");
        result.Should().BeNull();
    }

    [Fact]
    public async Task ExtractAndSaveAsync_WithNoExtractedSkills_CreatesEmployeeWithoutSkills()
    {
        // Arrange
        var context = TestDbFactory.CreateSqliteDbContext();

        var mockSkillExtractor = new Mock<ISkillExtractionService>();
        mockSkillExtractor
            .Setup(x => x.ExtractSkillsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Skill>());

        var service = new ExtractionService(context, mockSkillExtractor.Object);
        var dto = new ExtractRequestDto
        {
            FullName = "Bob",
            Email = null,
            RawText = "No skills mentioned here"
        };

        // Act
        var (ok, result, error) = await service.ExtractAndSaveAsync(dto, CancellationToken.None);

        // Assert
        ok.Should().BeTrue($"Expected extraction to succeed but got error: {error}");
        result?.ExtractedSkills.Should().BeEmpty();
        var employeeSkills = context.EmployeeSkills.Where(es => es.EmployeeId == result!.EmployeeId).ToList();
        employeeSkills.Should().BeEmpty();
    }

    [Fact]
    public async Task ExtractAndSaveAsync_CallsSkillExtractorWithRawText()
    {
        // Arrange
        var context = TestDbFactory.CreateSqliteDbContext();
        var mockSkillExtractor = new Mock<ISkillExtractionService>();
        var rawText = "C# .NET React";

        mockSkillExtractor
            .Setup(x => x.ExtractSkillsAsync(rawText, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Skill>());

        var service = new ExtractionService(context, mockSkillExtractor.Object);
        var dto = new ExtractRequestDto
        {
            FullName = "Charlie",
            Email = null,
            RawText = rawText
        };

        // Act
        var (ok, result, error) = await service.ExtractAndSaveAsync(dto, CancellationToken.None);

        // Assert
        ok.Should().BeTrue($"Expected extraction to succeed but got error: {error}");
        mockSkillExtractor.Verify(
            x => x.ExtractSkillsAsync(rawText, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
