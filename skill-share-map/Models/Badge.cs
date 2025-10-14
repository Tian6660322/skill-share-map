namespace SkillShareMap.Models;

// Badge tiers based on XP thresholds
public enum BadgeTier
{
    Newbie,    // XP < 50
    Skilled,   // XP >= 50
    Advanced,  // XP >= 200
    Expert,    // XP >= 600
    Master     // XP >= 1500
}

// User's earned badges
public class UserBadge
{
    public int Id { get; set; }

    // User reference
    public int UserId { get; set; }
    public User? User { get; set; }

    // Category
    public TaskCategory Category { get; set; }

    // Badge tier
    public BadgeTier Tier { get; set; }

    // When earned
    public DateTime EarnedAt { get; set; } = DateTime.UtcNow;
}
