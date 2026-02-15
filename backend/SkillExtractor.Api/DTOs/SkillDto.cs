using System;

namespace SkillExtractor.Api.DTOs
{
    public class SkillDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Aliases { get; set; }
    }
}
