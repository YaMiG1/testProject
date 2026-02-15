using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SkillExtractor.Api.Data;
using SkillExtractor.Api.Models;

namespace SkillExtractor.Api.Services
{
    public class SkillExtractionService : ISkillExtractionService
    {
        private readonly AppDbContext _db;

        public SkillExtractionService(AppDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task<List<Skill>> ExtractSkillsAsync(string rawText, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(rawText))
                return new List<Skill>();

            // Load skills from DB (Id, Name, Aliases)
            var skills = await _db.Skills
                .Select(s => new Skill { Id = s.Id, Name = s.Name, Aliases = s.Aliases })
                .ToListAsync(ct);

            // Build token sets from rawText
            // Keep letters, digits, '#' '.' '+' as part of tokens; split on anything else
            var splitter = new Regex("[^A-Za-z0-9#\\.\\+]+", RegexOptions.Compiled);

            var lowered = rawText.ToLowerInvariant();
            var rawTokens = splitter.Split(lowered)
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Distinct();

            var tokenSet = new HashSet<string>(rawTokens);

            // Secondary set: tokens with dots removed (e.g. ".net" -> "net")
            var tokenSetNoDots = new HashSet<string>(rawTokens.Select(t => t.Replace(".", string.Empty)));

            var matched = new Dictionary<int, Skill>();

            foreach (var skill in skills)
            {
                // Build candidate terms: Name + Aliases (aliases split by comma)
                var candidates = new List<string>();
                if (!string.IsNullOrWhiteSpace(skill.Name))
                    candidates.Add(skill.Name);

                if (!string.IsNullOrWhiteSpace(skill.Aliases))
                {
                    var parts = skill.Aliases.Split(',');
                    foreach (var p in parts)
                    {
                        if (!string.IsNullOrWhiteSpace(p))
                            candidates.Add(p);
                    }
                }

                var isMatch = false;

                foreach (var cand in candidates)
                {
                    var normalized = cand.ToLowerInvariant().Trim();
                    if (string.IsNullOrEmpty(normalized))
                        continue;

                    // Direct match: the whole candidate appears as a token
                    if (tokenSet.Contains(normalized))
                    {
                        isMatch = true;
                        break;
                    }

                    // Candidate without dots
                    var withoutDots = normalized.Replace(".", string.Empty);
                    if (tokenSetNoDots.Contains(withoutDots))
                    {
                        isMatch = true;
                        break;
                    }

                    // Also check candidate split into tokens (e.g. "asp.net core" -> "asp.net", "core")
                    var candTokens = splitter.Split(normalized).Where(t => !string.IsNullOrWhiteSpace(t));
                    foreach (var ctok in candTokens)
                    {
                        if (tokenSet.Contains(ctok) || tokenSetNoDots.Contains(ctok.Replace(".", string.Empty)))
                        {
                            isMatch = true;
                            break;
                        }
                    }

                    if (isMatch)
                        break;
                }

                if (isMatch && !matched.ContainsKey(skill.Id))
                    matched[skill.Id] = skill;
            }

            return matched.Values.ToList();
        }
    }
}
