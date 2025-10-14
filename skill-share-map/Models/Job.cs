namespace SkillShareMap.Models;

// Job postings for companies and schools
public class Job
{
    public int Id { get; set; }

    // Job details
    public string Title { get; set; } = string.Empty;
    public string Responsibilities { get; set; } = string.Empty;
    public string Qualifications { get; set; } = string.Empty;

    // Employment type
    public EmploymentType EmploymentType { get; set; }

    // Posted by
    public int PostedById { get; set; }
    public User? PostedBy { get; set; }

    // Location
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? LocationAddress { get; set; }

    // Status
    public bool IsOpen { get; set; } = true;

    // Timestamps
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ClosedAt { get; set; }
}

public enum EmploymentType
{
    FullTime,
    PartTime,
    Internship,
    Contract,
    Temporary
}
