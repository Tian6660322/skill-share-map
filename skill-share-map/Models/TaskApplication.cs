namespace SkillShareMap.Models;

public class TaskApplication
{
    public int Id { get; set; }

    public int TaskId { get; set; }
    public SkillTask? Task { get; set; }

    public int ApplicantId { get; set; }
    public User? Applicant { get; set; }

    public decimal ProposedPrice { get; set; }

    public string Message { get; set; } = string.Empty;

    public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public enum ApplicationStatus
{
    Pending,
    Accepted,
    Rejected,
    Withdrawn
}