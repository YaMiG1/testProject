using SkillExtractor.Api.DTOs;

namespace SkillExtractor.Api.Services;

/// <summary>
/// Validates extraction requests (SRP: validation logic extracted from ExtractionService).
/// </summary>
public interface IExtractionValidator
{
    /// <summary>
    /// Validates the extraction request and returns error if invalid.
    /// </summary>
    /// <returns>Error message if validation fails; null if valid</returns>
    string? Validate(ExtractRequestDto? dto);
}

public class ExtractionValidator : IExtractionValidator
{
    public string? Validate(ExtractRequestDto? dto)
    {
        if (dto == null)
            return "validation";

        if (string.IsNullOrWhiteSpace(dto.FullName))
            return "validation";

        if (string.IsNullOrWhiteSpace(dto.RawText))
            return "validation";

        return null;
    }
}
