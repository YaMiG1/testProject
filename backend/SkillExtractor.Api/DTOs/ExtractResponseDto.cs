using System.Collections.Generic;

namespace SkillExtractor.Api.DTOs
{
    public class ExtractResponseDto
    {
        public int EmployeeId { get; set; }
        public List<SkillDto> ExtractedSkills { get; set; } = new List<SkillDto>();
    }
}
