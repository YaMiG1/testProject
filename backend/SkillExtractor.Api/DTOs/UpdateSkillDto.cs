namespace SkillExtractor.Api.DTOs
{
    public class UpdateSkillDto
    {
        public string Name { get; set; } = null!;
        public string? Aliases { get; set; }
    }
}
