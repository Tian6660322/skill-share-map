namespace SkillShareMap.Models;

// Courses and workshops for The Classroom
public class Course
{
    public int Id { get; set; }

    // Course details
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string? InstructorName { get; set; }

    // Category
    public TaskCategory Category { get; set; }

    // Course type
    public CourseType Type { get; set; }

    // External link (if applicable)
    public string? ExternalUrl { get; set; }

    // Duration and difficulty
    public string? Duration { get; set; } // e.g., "2 hours", "4 weeks"
    public DifficultyLevel Difficulty { get; set; }

    // Timestamps
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public enum CourseType
{
    OnlineCourse,
    Workshop,
    Tutorial,
    WebSeminar
}

public enum DifficultyLevel
{
    Beginner,
    Intermediate,
    Advanced,
    Expert
}
