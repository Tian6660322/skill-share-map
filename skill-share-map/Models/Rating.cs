namespace SkillShareMap.Models;

// Rating and review system
public class Rating
{
    public int Id { get; set; }

    // Rating value (1-5 stars)
    public int Stars { get; set; }

    // Written comment (required for XP to be awarded)
    public string Comment { get; set; } = string.Empty;

    // Who gave the rating
    public int FromUserId { get; set; }
    public User? FromUser { get; set; }

    // Who received the rating
    public int ToUserId { get; set; }
    public User? ToUser { get; set; }

    // Related task (if applicable)
    public int? TaskId { get; set; }
    public SkillTask? Task { get; set; }

    // Task category (for XP calculation)
    public TaskCategory? Category { get; set; }

    // XP awarded (calculated based on stars)
    public int XpAwarded { get; set; } = 0;

    // Timestamp
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
