namespace SkillShareMap.Models;

// Tracks user's XP and progress in each skill category
public class UserSkillProgress
{
    public int Id { get; set; }

    // User reference
    public int UserId { get; set; }
    public User? User { get; set; }

    // Category
    public TaskCategory Category { get; set; }

    // Total XP accumulated in this category
    public int TotalXp { get; set; } = 0;

    // Current badge tier
    public BadgeTier CurrentTier { get; set; } = BadgeTier.Newbie;

    // Timestamp
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}
