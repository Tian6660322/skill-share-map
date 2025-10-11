namespace SkillShareMap.Models.DTOs;

// Data needed when a user logs in
public class LoginDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}