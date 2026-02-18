using FluentAssertions;
using SkillExtractor.Api.DTOs;
using SkillExtractor.Api.Models;
using SkillExtractor.Api.Services;
using Xunit;

namespace SkillExtractor.Api.Tests;

public class SkillsServiceTests
{
    [Fact]
    public async Task CreateAsync_WithValidInput_CreatesSkillSuccessfully()
    {
        // Arrange
        var context = TestDbFactory.CreateDbContext();
        var service = TestServiceFactory.CreateSkillsServiceFromContext(context);
        var dto = new CreateSkillDto { Name = "C#", Aliases = "csharp" };

        // Act
        var (ok, result, error) = await service.CreateAsync(dto, CancellationToken.None);

        // Assert
        ok.Should().BeTrue();
        error.Should().BeNull();
        result.Should().NotBeNull();
        result!.Name.Should().Be("C#");
        result.Aliases.Should().Be("csharp");
    }

    [Fact]
    public async Task CreateAsync_TrimsWhitespace_FromNameAndAliases()
    {
        // Arrange
        var context = TestDbFactory.CreateDbContext();
        var service = TestServiceFactory.CreateSkillsServiceFromContext(context);
        var dto = new CreateSkillDto { Name = "  C#  ", Aliases = "  csharp  " };

        // Act
        var (ok, result, error) = await service.CreateAsync(dto, CancellationToken.None);

        // Assert
        ok.Should().BeTrue();
        result?.Name.Should().Be("C#");
        result?.Aliases.Should().Be("csharp");
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateName_CaseInsensitive_ReturnsDuplicateError()
    {
        // Arrange
        var context = TestDbFactory.CreateDbContextWithSeed(ctx =>
        {
            ctx.Skills.Add(new Skill { Name = "C#", Aliases = null });
        });

        var service = TestServiceFactory.CreateSkillsServiceFromContext(context);
        var dto = new CreateSkillDto { Name = "c#", Aliases = null };

        // Act
        var (ok, result, error) = await service.CreateAsync(dto, CancellationToken.None);

        // Assert
        ok.Should().BeFalse();
        error.Should().Be("duplicate");
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_WithNullOrEmptyName_ReturnsValidationError()
    {
        // Arrange
        var context = TestDbFactory.CreateDbContext();
        var service = TestServiceFactory.CreateSkillsServiceFromContext(context);

        // Act - null name
        var (ok1, result1, error1) = await service.CreateAsync(
            new CreateSkillDto { Name = null!, Aliases = null },
            CancellationToken.None);

        // Act - empty name
        var (ok2, result2, error2) = await service.CreateAsync(
            new CreateSkillDto { Name = "", Aliases = null },
            CancellationToken.None);

        // Act - whitespace name
        var (ok3, result3, error3) = await service.CreateAsync(
            new CreateSkillDto { Name = "   ", Aliases = null },
            CancellationToken.None);

        // Assert
        ok1.Should().BeFalse();
        ok2.Should().BeFalse();
        ok3.Should().BeFalse();
        error1.Should().Be("Name is required");
        error2.Should().Be("Name is required");
        error3.Should().Be("Name is required");
    }

    [Fact]
    public async Task CreateAsync_WithNullRequest_ReturnsError()
    {
        // Arrange
        var context = TestDbFactory.CreateDbContext();
        var service = TestServiceFactory.CreateSkillsServiceFromContext(context);

        // Act
        var (ok, result, error) = await service.CreateAsync(null!, CancellationToken.None);

        // Assert
        ok.Should().BeFalse();
        error.Should().Be("Request is null");
    }

    [Fact]
    public async Task UpdateAsync_WithValidChanges_UpdatesSkillSuccessfully()
    {
        // Arrange
        var context = TestDbFactory.CreateDbContextWithSeed(ctx =>
        {
            ctx.Skills.Add(new Skill { Name = "C#", Aliases = "csharp" });
        });

        var service = TestServiceFactory.CreateSkillsServiceFromContext(context);
        var skillId = context.Skills.First().Id;
        var dto = new UpdateSkillDto { Name = "C# Advanced", Aliases = "csharp,cs" };

        // Act
        var (ok, result, error, notFound) = await service.UpdateAsync(skillId, dto, CancellationToken.None);

        // Assert
        ok.Should().BeTrue();
        notFound.Should().BeFalse();
        error.Should().BeNull();
        result?.Name.Should().Be("C# Advanced");
        result?.Aliases.Should().Be("csharp,cs");
    }

    [Fact]
    public async Task UpdateAsync_PreventsDuplicates_CaseInsensitive()
    {
        // Arrange
        var context = TestDbFactory.CreateDbContextWithSeed(ctx =>
        {
            ctx.Skills.Add(new Skill { Name = "C#", Aliases = null });
            ctx.Skills.Add(new Skill { Name = "JavaScript", Aliases = null });
        });

        var service = TestServiceFactory.CreateSkillsServiceFromContext(context);
        var jsSkill = context.Skills.First(s => s.Name == "JavaScript");

        // Act - try to update JavaScript to C#
        var (ok, result, error, notFound) = await service.UpdateAsync(
            jsSkill.Id,
            new UpdateSkillDto { Name = "c#", Aliases = null },
            CancellationToken.None);

        // Assert
        ok.Should().BeFalse();
        error.Should().Be("duplicate");
        notFound.Should().BeFalse();
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        var context = TestDbFactory.CreateDbContext();
        var service = TestServiceFactory.CreateSkillsServiceFromContext(context);

        // Act
        var (ok, result, error, notFound) = await service.UpdateAsync(
            999,
            new UpdateSkillDto { Name = "C#", Aliases = null },
            CancellationToken.None);

        // Assert
        ok.Should().BeFalse();
        notFound.Should().BeTrue();
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WithExistingId_DeletesSkillSuccessfully()
    {
        // Arrange
        var context = TestDbFactory.CreateDbContextWithSeed(ctx =>
        {
            ctx.Skills.Add(new Skill { Name = "C#", Aliases = null });
        });

        var service = TestServiceFactory.CreateSkillsServiceFromContext(context);
        var skillId = context.Skills.First().Id;

        // Act
        var (ok, notFound) = await service.DeleteAsync(skillId, CancellationToken.None);

        // Assert
        ok.Should().BeTrue();
        notFound.Should().BeFalse();
        context.Skills.Should().BeEmpty();
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        var context = TestDbFactory.CreateDbContext();
        var service = TestServiceFactory.CreateSkillsServiceFromContext(context);

        // Act
        var (ok, notFound) = await service.DeleteAsync(999, CancellationToken.None);

        // Assert
        ok.Should().BeFalse();
        notFound.Should().BeTrue();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllSkillsOrderedByName()
    {
        // Arrange
        var context = TestDbFactory.CreateDbContextWithSeed(ctx =>
        {
            ctx.Skills.Add(new Skill { Name = "Zebra", Aliases = null });
            ctx.Skills.Add(new Skill { Name = "Apple", Aliases = null });
            ctx.Skills.Add(new Skill { Name = "Banana", Aliases = null });
        });

        var service = TestServiceFactory.CreateSkillsServiceFromContext(context);

        // Act
        var result = await service.GetAllAsync(CancellationToken.None);

        // Assert
        result.Should().HaveCount(3);
        result.Select(s => s.Name).Should().Equal("Apple", "Banana", "Zebra");
    }

    [Fact]
    public async Task GetAllAsync_WithEmptyDatabase_ReturnsEmptyList()
    {
        // Arrange
        var context = TestDbFactory.CreateDbContext();
        var service = TestServiceFactory.CreateSkillsServiceFromContext(context);

        // Act
        var result = await service.GetAllAsync(CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }
}
