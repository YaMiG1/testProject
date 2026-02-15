namespace SkillExtractor.Api.Models;

public class Skill
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Aliases { get; set; }

    // Navigation properties
    public ICollection<EmployeeSkill> EmployeeSkills { get; set; } = new List<EmployeeSkill>();
}
