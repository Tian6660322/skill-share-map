namespace SkillShareMap.Models;

// This class represents skills that users can have
public class Skill
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public SkillLevel Level { get; set; } = SkillLevel.Beginner;
}

// Skill levels from beginner to expert
public enum SkillLevel
{
    Beginner,
    Intermediate,
    Advanced,
    Expert
}
