namespace SkillShareMap.Models;

// This class represents tasks that users can create and complete
public class SkillTask
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // Task category (one of the 6 main categories)
    public TaskCategory Category { get; set; }

    // Status tracking
    public SkillTaskStatus Status { get; set; } = SkillTaskStatus.Open;

    // Timestamps
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? Deadline { get; set; }
    public DateTime? CompletedAt { get; set; }

    // Users involved
    public int CreatorId { get; set; }
    public User? Creator { get; set; }
    public int? AssignedToId { get; set; }
    public User? AssignedTo { get; set; }

    // Location
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? LocationAddress { get; set; }

    // Budget/Payment
    public decimal Budget { get; set; } = 0;
    public decimal? NegotiatedPrice { get; set; }
    public decimal? DepositAmount { get; set; }
    public bool IsDepositPaid { get; set; } = false;

    // Task completion and rating
    public bool IsCompleted { get; set; } = false;
    public bool IsHelperConfirmedComplete { get; set; } = false;
    public bool IsCreatorConfirmedComplete { get; set; } = false;
    public DateTime? HelperConfirmedAt { get; set; }
    public DateTime? CreatorConfirmedAt { get; set; }
    public Rating? Rating { get; set; }
}

// Task statuses to track progress
public enum SkillTaskStatus
{
    Open,                    // Available for anyone to accept
    Assigned,                // Assigned to someone, in progress
    PendingDeposit,          // Waiting for deposit payment
    PendingCompletion,       // Helper marked as done, waiting for creator confirmation
    Completed,               // Task completed successfully by both parties
    Cancelled                // Task cancelled by creator
}
