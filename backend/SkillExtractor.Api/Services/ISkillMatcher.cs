using System.Threading;
using System.Threading.Tasks;

namespace SkillExtractor.Api.Services;

/// <summary>
/// Responsible for matching candidate skill names/aliases against tokenized text.
/// Extracted from SkillExtractionService to follow SRP and improve testability.
/// </summary>
public interface ISkillMatcher
{
    /// <summary>
    /// Determines if any of the given candidates match the provided text tokens.
    /// </summary>
    /// <param name="candidates">Skill names and aliases to match against</param>
    /// <param name="textTokens">Tokens extracted from raw CV text</param>
    /// <param name="textTokensNoDots">Tokens with dots removed (e.g., ".net" -> "net")</param>
    /// <returns>True if any candidate matches the token sets</returns>
    bool IsMatch(IEnumerable<string> candidates, ISet<string> textTokens, ISet<string> textTokensNoDots);
}
