using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SkillExtractor.Api.DTOs;

namespace SkillExtractor.Api.Services
{
    public interface ISkillsService
    {
        Task<List<SkillDto>> GetAllAsync(CancellationToken ct);
        Task<(bool ok, SkillDto? result, string? error)> CreateAsync(CreateSkillDto req, CancellationToken ct);
        Task<(bool ok, SkillDto? result, string? error, bool notFound)> UpdateAsync(int id, UpdateSkillDto req, CancellationToken ct);
        Task<(bool ok, bool notFound)> DeleteAsync(int id, CancellationToken ct);
    }
}
