namespace SkillShareMap.Models;

// This class represents tasks that users can create and complete
public class SkillTask
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public SkillTaskStatus Status { get; set; } = SkillTaskStatus.Open;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public int CreatorId { get; set; }
    public User? Creator { get; set; }
    public int? AssignedToId { get; set; }
    public User? AssignedTo { get; set; }
    public List<Skill> RequiredSkills { get; set; } = new();
    public int RewardPoints { get; set; } = 10;
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}

// Task statuses to track progress
public enum SkillTaskStatus
{
    Open,
    Assigned,
    InProgress,
    Completed,
    Cancelled
}
