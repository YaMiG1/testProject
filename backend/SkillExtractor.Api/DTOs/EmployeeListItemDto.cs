using System;

namespace SkillExtractor.Api.DTOs
{
    public class EmployeeListItemDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string? Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public int SkillsCount { get; set; }
    }
}
