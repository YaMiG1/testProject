namespace SkillExtractor.Api.Models;

public class Employee
{
    public int Id { get; set; }
    public string FullName { get; set; } = null!;
    public string? Email { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public ICollection<CVDocument> CVDocuments { get; set; } = new List<CVDocument>();
    public ICollection<EmployeeSkill> EmployeeSkills { get; set; } = new List<EmployeeSkill>();

    public Employee()
    {
        CreatedAt = DateTime.UtcNow;
    }
}
