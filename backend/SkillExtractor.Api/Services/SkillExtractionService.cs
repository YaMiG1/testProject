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
        private readonly IRepository<Skill> _skillRepository;
        private readonly ISkillMatcher _skillMatcher;
        private readonly Regex _splitter = new Regex("[^A-Za-z0-9#\\.\\+]+", RegexOptions.Compiled);

        public SkillExtractionService(IRepository<Skill> skillRepository, ISkillMatcher skillMatcher)
        {
            _skillRepository = skillRepository ?? throw new ArgumentNullException(nameof(skillRepository));
            _skillMatcher = skillMatcher ?? throw new ArgumentNullException(nameof(skillMatcher));
        }

        public async Task<List<Skill>> ExtractSkillsAsync(string rawText, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(rawText))
                return new List<Skill>();

            // Load skills from repository
            var skills = await _skillRepository.Query()
                .Select(s => new Skill { Id = s.Id, Name = s.Name, Aliases = s.Aliases })
                .ToListAsync(ct);

            if (skills.Count == 0)
                return new List<Skill>();

            // Build token sets from rawText
            var lowered = rawText.ToLowerInvariant();
            var rawTokens = _splitter.Split(lowered)
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Distinct();

            var tokenSet = new HashSet<string>(rawTokens);
            var tokenSetNoDots = new HashSet<string>(rawTokens.Select(t => t.Replace(".", string.Empty)));

            var matched = new Dictionary<int, Skill>();

            foreach (var skill in skills)
            {
                // Build candidate terms: Name + Aliases (aliases split by comma)
                var candidates = BuildCandidates(skill.Name, skill.Aliases);

                // Delegate matching logic to matcher (SRP: separation of text matching from data access)
                if (_skillMatcher.IsMatch(candidates, tokenSet, tokenSetNoDots))
                {
                    matched[skill.Id] = skill;
                }
            }

            return matched.Values.ToList();
        }

        private static List<string> BuildCandidates(string? name, string? aliases)
        {
            var candidates = new List<string>();

            if (!string.IsNullOrWhiteSpace(name))
                candidates.Add(name);

            if (!string.IsNullOrWhiteSpace(aliases))
            {
                var parts = aliases.Split(',');
                foreach (var p in parts)
                {
                    if (!string.IsNullOrWhiteSpace(p))
                        candidates.Add(p);
                }
            }

            return candidates;
        }
    }
}

