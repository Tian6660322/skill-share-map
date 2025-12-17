using SkillShareMap.Models;
namespace SkillShareMap.Services;

public interface IAuthService
{
    Task<User?> RegisterAsync(User user, string password);
    Task<User?> LoginAsync(string username, string password);
    Task<User?> GetUserByIdAsync(int userId);
    Task<bool> CheckIdentifierExistsAsync(string identifier);
    Task<bool> UpdateUserAsync(User user);
    Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword);
}

