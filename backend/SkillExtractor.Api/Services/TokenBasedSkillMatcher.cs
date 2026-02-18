using System.Text.RegularExpressions;

namespace SkillExtractor.Api.Services;

/// <summary>
/// Implements skill matching logic extracted from SkillExtractionService.
/// Responsible ONLY for text matching algorithm; delegates data access to caller.
/// </summary>
public class TokenBasedSkillMatcher : ISkillMatcher
{
    private readonly Regex _splitter = new Regex("[^A-Za-z0-9#\\.\\+]+", RegexOptions.Compiled);

    public bool IsMatch(IEnumerable<string> candidates, ISet<string> textTokens, ISet<string> textTokensNoDots)
    {
        foreach (var cand in candidates)
        {
            var normalized = cand.ToLowerInvariant().Trim();
            if (string.IsNullOrEmpty(normalized))
                continue;

            // Direct match: the whole candidate appears as a token
            if (textTokens.Contains(normalized))
                return true;

            // Candidate without dots (for ".NET" matching "net")
            var withoutDots = normalized.Replace(".", string.Empty);
            if (textTokensNoDots.Contains(withoutDots))
                return true;

            // Check candidate split into tokens (e.g., "asp.net core" -> ["asp.net", "core"])
            var candTokens = _splitter.Split(normalized).Where(t => !string.IsNullOrWhiteSpace(t));
            foreach (var ctok in candTokens)
            {
                if (textTokens.Contains(ctok) || textTokensNoDots.Contains(ctok.Replace(".", string.Empty)))
                    return true;
            }
        }

        return false;
    }
}
