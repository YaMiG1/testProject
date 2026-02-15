namespace SkillExtractor.Api.DTOs
{
    public class CreateSkillDto
    {
        public string Name { get; set; } = null!;
        public string? Aliases { get; set; }
    }
}
