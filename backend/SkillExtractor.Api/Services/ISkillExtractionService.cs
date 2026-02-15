using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SkillExtractor.Api.Models;

namespace SkillExtractor.Api.Services
{
    public interface ISkillExtractionService
    {
        Task<List<Skill>> ExtractSkillsAsync(string rawText, CancellationToken ct);
    }
}
