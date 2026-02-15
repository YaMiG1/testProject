using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SkillExtractor.Api.Data;
using SkillExtractor.Api.DTOs;
using SkillExtractor.Api.Models;

namespace SkillExtractor.Api.Services
{
    public class ExtractionService : IExtractionService
    {
        private readonly AppDbContext _db;
        private readonly ISkillExtractionService _skillExtractor;

        public ExtractionService(AppDbContext db, ISkillExtractionService skillExtractor)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _skillExtractor = skillExtractor ?? throw new ArgumentNullException(nameof(skillExtractor));
        }

        public async Task<(bool ok, ExtractResponseDto? result, string? error)> ExtractAndSaveAsync(ExtractRequestDto dto, CancellationToken ct)
        {
            // Validate inputs
            if (dto == null || string.IsNullOrWhiteSpace(dto.FullName) || string.IsNullOrWhiteSpace(dto.RawText))
                return (false, null, "validation");

            try
            {
                // Use transaction for consistency
                using var transaction = await _db.Database.BeginTransactionAsync(ct);

                try
                {
                    // Create Employee
                    var employee = new Employee
                    {
                        FullName = dto.FullName.Trim(),
                        Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim()
                    };
                    _db.Employees.Add(employee);
                    await _db.SaveChangesAsync(ct);

                    // Create CVDocument
                    var cvDoc = new CVDocument
                    {
                        EmployeeId = employee.Id,
                        RawText = dto.RawText
                    };
                    _db.CVDocuments.Add(cvDoc);
                    await _db.SaveChangesAsync(ct);

                    // Extract skills using SkillExtractionService
                    var matchedSkills = await _skillExtractor.ExtractSkillsAsync(dto.RawText, ct);

                    // Create EmployeeSkill join rows for distinct matched skills
                    var distinctSkillIds = matchedSkills.Select(s => s.Id).Distinct().ToList();
                    foreach (var skillId in distinctSkillIds)
                    {
                        var employeeSkill = new EmployeeSkill
                        {
                            EmployeeId = employee.Id,
                            SkillId = skillId
                        };
                        _db.EmployeeSkills.Add(employeeSkill);
                    }
                    await _db.SaveChangesAsync(ct);

                    // Commit transaction
                    await transaction.CommitAsync(ct);

                    // Build response
                    var response = new ExtractResponseDto
                    {
                        EmployeeId = employee.Id,
                        ExtractedSkills = matchedSkills.Select(s => s.ToDto()).ToList()
                    };

                    return (true, response, null);
                }
                catch
                {
                    await transaction.RollbackAsync(ct);
                    throw;
                }
            }
            catch (Exception ex)
            {
                return (false, null, ex.Message);
            }
        }
    }
}
