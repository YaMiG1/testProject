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
    /// <summary>
    /// Orchestrates the end-to-end extraction workflow (SRP: thin orchestrator).
    /// Delegates to repositories and service abstractions; keeps behavior unchanged.
    /// </summary>
    public class ExtractionService : IExtractionService
    {
        private readonly AppDbContext _db;
        private readonly ISkillExtractionService _skillExtractor;
        private readonly IExtractionValidator _validator;
        private readonly IRepository<Employee> _employeeRepository;
        private readonly IRepository<CVDocument> _cvDocumentRepository;
        private readonly IRepository<EmployeeSkill> _employeeSkillRepository;

        public ExtractionService(
            AppDbContext db,
            ISkillExtractionService skillExtractor,
            IExtractionValidator validator,
            IRepository<Employee> employeeRepository,
            IRepository<CVDocument> cvDocumentRepository,
            IRepository<EmployeeSkill> employeeSkillRepository)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _skillExtractor = skillExtractor ?? throw new ArgumentNullException(nameof(skillExtractor));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
            _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
            _cvDocumentRepository = cvDocumentRepository ?? throw new ArgumentNullException(nameof(cvDocumentRepository));
            _employeeSkillRepository = employeeSkillRepository ?? throw new ArgumentNullException(nameof(employeeSkillRepository));
        }

        public async Task<(bool ok, ExtractResponseDto? result, string? error)> ExtractAndSaveAsync(ExtractRequestDto dto, CancellationToken ct)
        {
            // Validate inputs (SRP: validation delegated to validator)
            var validationError = _validator.Validate(dto);
            if (validationError != null)
                return (false, null, validationError);

            try
            {
                // Use transaction for consistency
                using var transaction = await _db.Database.BeginTransactionAsync(ct);

                try
                {
                    // Create Employee (SRP: entity creation via repositories)
                    var employee = CreateEmployee(dto);
                    _employeeRepository.Add(employee);
                    await _employeeRepository.SaveChangesAsync(ct);

                    // Create CVDocument
                    var cvDoc = CreateCvDocument(employee.Id, dto.RawText);
                    _cvDocumentRepository.Add(cvDoc);
                    await _cvDocumentRepository.SaveChangesAsync(ct);

                    // Extract skills using SkillExtractionService
                    var matchedSkills = await _skillExtractor.ExtractSkillsAsync(dto.RawText, ct);

                    // Create EmployeeSkill join rows for distinct matched skills
                    var distinctSkillIds = matchedSkills.Select(s => s.Id).Distinct().ToList();
                    foreach (var skillId in distinctSkillIds)
                    {
                        var employeeSkill = CreateEmployeeSkill(employee.Id, skillId);
                        _employeeSkillRepository.Add(employeeSkill);
                    }
                    await _employeeSkillRepository.SaveChangesAsync(ct);

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

        /// <summary>
        /// Factory method for creating an Employee entity (OCP: extraction of entity creation).
        /// </summary>
        private static Employee CreateEmployee(ExtractRequestDto dto)
        {
            return new Employee
            {
                FullName = dto.FullName.Trim(),
                Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim()
            };
        }

        /// <summary>
        /// Factory method for creating a CVDocument entity.
        /// </summary>
        private static CVDocument CreateCvDocument(int employeeId, string rawText)
        {
            return new CVDocument
            {
                EmployeeId = employeeId,
                RawText = rawText
            };
        }

        /// <summary>
        /// Factory method for creating an EmployeeSkill entity.
        /// </summary>
        private static EmployeeSkill CreateEmployeeSkill(int employeeId, int skillId)
        {
            return new EmployeeSkill
            {
                EmployeeId = employeeId,
                SkillId = skillId
            };
        }
    }
}
