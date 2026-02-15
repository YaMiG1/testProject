using System;

namespace SkillExtractor.Api.DTOs
{
    public class CvDocumentDto
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Preview { get; set; } = string.Empty;
    }
}
