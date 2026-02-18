using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SkillExtractor.Api.Data;
using SkillExtractor.Api.DTOs;

namespace SkillExtractor.Api.Services
{
    /// <summary>
    /// Provides query operations for employees (SRP: delegates to repository for data access).
    /// </summary>
    public class EmployeesService : IEmployeesService
    {
        private readonly IRepository<Models.Employee> _employeeRepository;

        public EmployeesService(IRepository<Models.Employee> employeeRepository)
        {
            _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
        }

        public async Task<List<EmployeeListItemDto>> GetAllAsync(CancellationToken ct)
        {
            return await _employeeRepository.Query()
                .OrderBy(e => e.FullName)
                .Select(e => new EmployeeListItemDto
                {
                    Id = e.Id,
                    FullName = e.FullName,
                    Email = e.Email,
                    CreatedAt = e.CreatedAt,
                    SkillsCount = e.EmployeeSkills.Count()
                })
                .ToListAsync(ct);
        }

        public async Task<EmployeeDetailsDto?> GetByIdAsync(int id, CancellationToken ct)
        {
            var emp = await _employeeRepository.Query()
                .Where(e => e.Id == id)
                .Select(e => new
                {
                    e.Id,
                    e.FullName,
                    e.Email,
                    e.CreatedAt,
                    Skills = e.EmployeeSkills.Select(es => new { es.Skill.Id, es.Skill.Name, es.Skill.Aliases }),
                    CvDocs = e.CVDocuments.Select(cv => new { cv.Id, cv.CreatedAt, cv.RawText })
                })
                .FirstOrDefaultAsync(ct);

            if (emp == null) return null;

            var details = new EmployeeDetailsDto
            {
                Id = emp.Id,
                FullName = emp.FullName,
                Email = emp.Email,
                CreatedAt = emp.CreatedAt,
                Skills = emp.Skills.Select(s => new SkillDto { Id = s.Id, Name = s.Name, Aliases = s.Aliases }).ToList(),
                CvDocuments = emp.CvDocs.Select(cv => new CvDocumentDto
                {
                    Id = cv.Id,
                    CreatedAt = cv.CreatedAt,
                    Preview = string.IsNullOrEmpty(cv.RawText) ? string.Empty : (cv.RawText.Length > 200 ? cv.RawText.Substring(0, 200) : cv.RawText)
                }).ToList()
            };

            return details;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct)
        {
            var emp = await _employeeRepository.GetByIdAsync(id, ct);
            if (emp == null) return false;

            _employeeRepository.Remove(emp);
            await _employeeRepository.SaveChangesAsync(ct);
            return true;
        }
    }
}
