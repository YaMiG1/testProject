using FluentAssertions;
using SkillExtractor.Api.Models;
using SkillExtractor.Api.Services;
using Xunit;

namespace SkillExtractor.Api.Tests;

public class SkillExtractionServiceTests
{
    [Fact]
    public async Task ExtractSkillsAsync_WithCaseSensitiveText_ReturnMatchedSkillsCaseInsensitive()
    {
        // Arrange
        var context = TestDbFactory.CreateDbContextWithSeed(ctx =>
        {
            ctx.Skills.Add(new Skill { Name = "C#", Aliases = "csharp" });
            ctx.Skills.Add(new Skill { Name = ".NET", Aliases = "dotnet,net" });
            ctx.Skills.Add(new Skill { Name = "React", Aliases = "reactjs" });
        });

        var service = TestServiceFactory.CreateSkillExtractionServiceFromContext(context);
        var rawText = "C# .NET react";

        // Act
        var result = await service.ExtractSkillsAsync(rawText, CancellationToken.None);

        // Assert
        result.Should().HaveCount(3);
        result.Select(s => s.Name).Should().Contain(new[] { "C#", ".NET", "React" });
    }

    [Fact]
    public async Task ExtractSkillsAsync_WithAliases_MatchesSkillsByAlias()
    {
        // Arrange
        var context = TestDbFactory.CreateDbContextWithSeed(ctx =>
        {
            ctx.Skills.Add(new Skill { Name = "C#", Aliases = "csharp" });
            ctx.Skills.Add(new Skill { Name = ".NET", Aliases = "dotnet" });
        });

        var service = TestServiceFactory.CreateSkillExtractionServiceFromContext(context);
        var rawText = "csharp dotnet";

        // Act
        var result = await service.ExtractSkillsAsync(rawText, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result.Select(s => s.Name).Should().Contain(new[] { "C#", ".NET" });
    }

    [Fact]
    public async Task ExtractSkillsAsync_WithEmptyText_ReturnsEmptyList()
    {
        // Arrange
        var context = TestDbFactory.CreateDbContextWithSeed(ctx =>
        {
            ctx.Skills.Add(new Skill { Name = "C#", Aliases = "csharp" });
        });

        var service = TestServiceFactory.CreateSkillExtractionServiceFromContext(context);

        // Act - empty string
        var resultEmpty = await service.ExtractSkillsAsync(string.Empty, CancellationToken.None);

        // Act - whitespace only
        var resultWhitespace = await service.ExtractSkillsAsync("   ", CancellationToken.None);

        // Assert
        resultEmpty.Should().BeEmpty();
        resultWhitespace.Should().BeEmpty();
    }

    [Fact]
    public async Task ExtractSkillsAsync_WithNoMatches_ReturnsEmptyList()
    {
        // Arrange
        var context = TestDbFactory.CreateDbContextWithSeed(ctx =>
        {
            ctx.Skills.Add(new Skill { Name = "C#", Aliases = "csharp" });
        });

        var service = TestServiceFactory.CreateSkillExtractionServiceFromContext(context);
        var rawText = "Java Python Ruby";

        // Act
        var result = await service.ExtractSkillsAsync(rawText, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task ExtractSkillsAsync_WithDotNotationSkill_MatchesWithoutDot()
    {
        // Arrange
        var context = TestDbFactory.CreateDbContextWithSeed(ctx =>
        {
            ctx.Skills.Add(new Skill { Name = ".NET", Aliases = null });
            ctx.Skills.Add(new Skill { Name = "ASP.NET Core", Aliases = null });
        });

        var service = TestServiceFactory.CreateSkillExtractionServiceFromContext(context);
        // "net" and "aspnet core" without dots
        var rawText = "net aspnet core";

        // Act
        var result = await service.ExtractSkillsAsync(rawText, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result.Select(s => s.Name).Should().Contain(new[] { ".NET", "ASP.NET Core" });
    }

    [Fact]
    public async Task ExtractSkillsAsync_WithMultipleAliases_MatchesCorrectly()
    {
        // Arrange
        var context = TestDbFactory.CreateDbContextWithSeed(ctx =>
        {
            ctx.Skills.Add(new Skill { Name = "JavaScript", Aliases = "JS,JS,JavaScript ES6" });
        });

        var service = TestServiceFactory.CreateSkillExtractionServiceFromContext(context);
        var rawText = "js es6";

        // Act
        var result = await service.ExtractSkillsAsync(rawText, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result.First().Name.Should().Be("JavaScript");
    }

    [Fact]
    public async Task ExtractSkillsAsync_WithDuplicatesInText_ReturnsDuplicateSkillOnce()
    {
        // Arrange
        var context = TestDbFactory.CreateDbContextWithSeed(ctx =>
        {
            ctx.Skills.Add(new Skill { Name = "C#", Aliases = null });
        });

        var service = TestServiceFactory.CreateSkillExtractionServiceFromContext(context);
        var rawText = "C# C# C#";

        // Act
        var result = await service.ExtractSkillsAsync(rawText, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result.First().Name.Should().Be("C#");
    }
}
