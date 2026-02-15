using System.Threading;
using System.Threading.Tasks;
using SkillExtractor.Api.DTOs;

namespace SkillExtractor.Api.Services
{
    public interface IExtractionService
    {
        Task<(bool ok, ExtractResponseDto? result, string? error)> ExtractAndSaveAsync(ExtractRequestDto dto, CancellationToken ct);
    }
}
