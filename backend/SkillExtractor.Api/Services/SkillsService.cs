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
    public class SkillsService : ISkillsService
    {
        private readonly AppDbContext _db;

        public SkillsService(AppDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task<List<SkillDto>> GetAllAsync(CancellationToken ct)
        {
            var skills = await _db.Skills
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

            if (string.IsNullOrWhiteSpace(name))
                return (false, null, "Name is required");

            var nameLower = name.ToLowerInvariant();
            var exists = await _db.Skills.AnyAsync(s => s.Name.ToLower() == nameLower, ct);
            
            if (exists)
                return (false, null, "duplicate");

            var skill = new Skill
            {
                Name = name,
                Aliases = string.IsNullOrWhiteSpace(aliases) ? null : aliases
            };

            _db.Skills.Add(skill);
            await _db.SaveChangesAsync(ct);

            return (true, skill.ToDto(), null);
        }

        public async Task<(bool ok, SkillDto? result, string? error, bool notFound)> UpdateAsync(int id, UpdateSkillDto req, CancellationToken ct)
        {
            if (req == null)
                return (false, null, "Request is null", false);

            var name = req.Name?.Trim();
            var aliases = req.Aliases?.Trim();

            if (string.IsNullOrWhiteSpace(name))
                return (false, null, "Name is required", false);

            var skill = await _db.Skills.FindAsync(new object[] { id }, ct);
            if (skill == null)
                return (false, null, null, true);

            var nameLower = name.ToLowerInvariant();
            var duplicate = await _db.Skills.AnyAsync(s => s.Id != id && s.Name.ToLower() == nameLower, ct);
            
            if (duplicate)
                return (false, null, "duplicate", false);

            skill.Name = name;
            skill.Aliases = string.IsNullOrWhiteSpace(aliases) ? null : aliases;

            _db.Skills.Update(skill);
            await _db.SaveChangesAsync(ct);

            return (true, skill.ToDto(), null, false);
        }

        public async Task<(bool ok, bool notFound)> DeleteAsync(int id, CancellationToken ct)
        {
            var skill = await _db.Skills.FindAsync(new object[] { id }, ct);
            if (skill == null)
                return (false, true);

            _db.Skills.Remove(skill);
            await _db.SaveChangesAsync(ct);

            return (true, false);
        }
    }
}
