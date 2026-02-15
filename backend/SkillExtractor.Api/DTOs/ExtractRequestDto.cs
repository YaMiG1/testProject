namespace SkillExtractor.Api.DTOs
{
    public class ExtractRequestDto
    {
        public string FullName { get; set; } = null!;
        public string? Email { get; set; }
        public string RawText { get; set; } = null!;
    }
}
