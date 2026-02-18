using Microsoft.EntityFrameworkCore;
using SkillExtractor.Api.Data;
using SkillExtractor.Api.DTOs;
using SkillExtractor.Api.Models;

namespace SkillExtractor.Api.Services
{
    /// <summary>
    /// Provides CRUD operations for skills (SRP: thin service delegating to validators and repositories).
    /// </summary>
    public class SkillsService : ISkillsService
    {
        private readonly IRepository<Skill> _skillRepository;
        private readonly ISkillValidator _validator;

        public SkillsService(IRepository<Skill> skillRepository, ISkillValidator validator)
        {
            _skillRepository = skillRepository ?? throw new ArgumentNullException(nameof(skillRepository));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        public async Task<List<SkillDto>> GetAllAsync(CancellationToken ct)
        {
            var skills = await _skillRepository.Query()
                .OrderBy(s => s.Name)
                .ToListAsync(ct);

            return skills.Select(s => s.ToDto()).ToList();
        }

        public async Task<(bool ok, SkillDto? result, string? error)> CreateAsync(CreateSkillDto req, CancellationToken ct)
        {
            if (req == null)
                return (false, null, "Request is null");

            var name = req.Name?.Trim();
            var aliases = req.Aliases?.Trim();

            // Validate name (SRP: validation logic delegated to validator)
            var nameError = _validator.ValidateName(name);
            if (nameError != null)
                return (false, null, nameError);

            // Check for duplicate (SRP: validator checks duplicates)
            var exists = await _validator.NameExistsAsync(name!, ct: ct);
            if (exists)
                return (false, null, "duplicate");

            var skill = new Skill
            {
                Name = name!,
                Aliases = string.IsNullOrWhiteSpace(aliases) ? null : aliases
            };

            _skillRepository.Add(skill);
            await _skillRepository.SaveChangesAsync(ct);

            return (true, skill.ToDto(), null);
        }

        public async Task<(bool ok, SkillDto? result, string? error, bool notFound)> UpdateAsync(int id, UpdateSkillDto req, CancellationToken ct)
        {
            if (req == null)
                return (false, null, "Request is null", false);

            var name = req.Name?.Trim();
            var aliases = req.Aliases?.Trim();

            // Validate name
            var nameError = _validator.ValidateName(name);
            if (nameError != null)
                return (false, null, nameError, false);

            var skill = await _skillRepository.GetByIdAsync(id, ct);
            if (skill == null)
                return (false, null, null, true);

            // Check for duplicate (excluding current skill)
            var duplicate = await _validator.NameExistsAsync(name!, excludeId: id, ct: ct);
            if (duplicate)
                return (false, null, "duplicate", false);

            skill.Name = name!;
            skill.Aliases = string.IsNullOrWhiteSpace(aliases) ? null : aliases;

            _skillRepository.Update(skill);
            await _skillRepository.SaveChangesAsync(ct);

            return (true, skill.ToDto(), null, false);
        }

        public async Task<(bool ok, bool notFound)> DeleteAsync(int id, CancellationToken ct)
        {
            var skill = await _skillRepository.GetByIdAsync(id, ct);
            if (skill == null)
                return (false, true);

            _skillRepository.Remove(skill);
            await _skillRepository.SaveChangesAsync(ct);

            return (true, false);
        }
    }
}
