using Microsoft.EntityFrameworkCore;
using SkillExtractor.Api.DTOs;

namespace SkillExtractor.Api.Services;

/// <summary>
/// Validates skill creation/update requests (SRP: validation extracted from SkillsService).
/// </summary>
public interface ISkillValidator
{
    /// <summary>
    /// Validates a skill name.
    /// </summary>
    /// <returns>Error message if invalid; null if valid</returns>
    string? ValidateName(string? name);

    /// <summary>
    /// Checks if a skill name already exists (case-insensitive).
    /// </summary>
    Task<bool> NameExistsAsync(string name, int? excludeId = null, CancellationToken ct = default);
}

public class SkillValidator : ISkillValidator
{
    private readonly IRepository<Models.Skill> _skillRepository;

    public SkillValidator(IRepository<Models.Skill> skillRepository)
    {
        _skillRepository = skillRepository ?? throw new ArgumentNullException(nameof(skillRepository));
    }

    public string? ValidateName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name?.Trim()))
            return "Name is required";

        return null;
    }

    public async Task<bool> NameExistsAsync(string name, int? excludeId = null, CancellationToken ct = default)
    {
        var nameLower = name.ToLowerInvariant();
        var query = _skillRepository.Query();

        if (excludeId.HasValue)
            query = query.Where(s => s.Id != excludeId);

        return await query.AnyAsync(s => s.Name.ToLower() == nameLower, ct);
    }
}
