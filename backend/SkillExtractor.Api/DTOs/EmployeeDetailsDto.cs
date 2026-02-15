using System;
using System.Collections.Generic;

namespace SkillExtractor.Api.DTOs
{
    public class EmployeeDetailsDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string? Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<SkillDto> Skills { get; set; } = new List<SkillDto>();
        public List<CvDocumentDto> CvDocuments { get; set; } = new List<CvDocumentDto>();
    }
}
