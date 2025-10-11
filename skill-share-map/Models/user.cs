namespace SkillShareMap.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? Bio { get; set; }
    public string? AvatarUrl { get; set; }
    public List<Skill> Skills { get; set; } = new();
    public List<SkillTask> CreatedTasks { get; set; } = new();
    public List<SkillTask> AssignedTasks { get; set; } = new();
}
