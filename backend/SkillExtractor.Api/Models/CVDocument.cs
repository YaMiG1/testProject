namespace SkillExtractor.Api.Models;

public class CVDocument
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string RawText { get; set; } = null!;
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public Employee Employee { get; set; } = null!;

    public CVDocument()
    {
        CreatedAt = DateTime.UtcNow;
    }
}
