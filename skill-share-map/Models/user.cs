namespace SkillShareMap.Models;

public class User
{
    public int Id { get; set; }

    // Account credentials
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    // User role (Student, Company, School)
    public UserRole Role { get; set; } = UserRole.Student;

    // Common fields
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? Bio { get; set; }
    public string? AvatarUrl { get; set; }

    // Home base location for distance filtering
    public double? HomeBaseLatitude { get; set; }
    public double? HomeBaseLongitude { get; set; }
    public string? HomeBaseAddress { get; set; }

    // Student-specific fields
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? SchoolName { get; set; }
    public string? StudentCardUrl { get; set; } // URL to student card image
    public bool IsVerified { get; set; } = false;

    // Company/School-specific fields
    public string? CompanyName { get; set; }
    public string? WebsiteUrl { get; set; }
    public string? CompanyDescription { get; set; }

    // Selected skill categories during onboarding
    public List<TaskCategory> SelectedCategories { get; set; } = new();

    // Reputation level (1-5 stars based on average rating)
    public double ReputationLevel { get; set; } = 0;

    // Skills and XP tracking per category
    public List<UserSkillProgress> SkillProgress { get; set; } = new();

    // Badges earned per category
    public List<UserBadge> Badges { get; set; } = new();

    // Tasks
    public List<SkillTask> CreatedTasks { get; set; } = new();
    public List<SkillTask> AssignedTasks { get; set; } = new();

    // Jobs (for companies/schools)
    public List<Job> PostedJobs { get; set; } = new();

    // Wallet
    public Wallet? Wallet { get; set; }

    // Ratings received
    public List<Rating> ReceivedRatings { get; set; } = new();

    // Ratings given
    public List<Rating> GivenRatings { get; set; } = new();
}

public enum UserRole
{
    Student,
    Company,
    School
}
