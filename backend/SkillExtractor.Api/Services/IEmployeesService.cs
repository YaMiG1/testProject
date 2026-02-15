using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SkillExtractor.Api.DTOs;

namespace SkillExtractor.Api.Services
{
    public interface IEmployeesService
    {
        Task<List<EmployeeListItemDto>> GetAllAsync(CancellationToken ct);
        Task<EmployeeDetailsDto?> GetByIdAsync(int id, CancellationToken ct);
        Task<bool> DeleteAsync(int id, CancellationToken ct);
    }
}
