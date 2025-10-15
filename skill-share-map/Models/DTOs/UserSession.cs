namespace SkillShareMap.Models.DTOs;

/// <summary>
/// Simplified user data for session storage (avoiding circular references)
/// </summary>
public class UserSession
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? CompanyName { get; set; }
    public string? AvatarUrl { get; set; }
    public double ReputationLevel { get; set; }
    public bool IsVerified { get; set; }

    public static UserSession FromUser(User user)
    {
        return new UserSession
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role,
            FirstName = user.FirstName,
            LastName = user.LastName,
            CompanyName = user.CompanyName,
            AvatarUrl = user.AvatarUrl,
            ReputationLevel = user.ReputationLevel,
            IsVerified = user.IsVerified
        };
    }
}
