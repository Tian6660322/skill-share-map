using Microsoft.EntityFrameworkCore;
using SkillShareMap.Data;
using SkillShareMap.Models;

namespace SkillShareMap.Services;

public interface IAuthService
{
    Task<User?> RegisterAsync(User user, string password);
    Task<User?> LoginAsync(string username, string password);
    Task<User?> GetUserByIdAsync(int userId);
    Task<bool> UpdateUserAsync(User user);
}

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;

    public AuthService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    public async Task<User?> RegisterAsync(User user, string password)
    {
        // Check if username already exists
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == user.Username);

        if (existingUser != null)
            return null;

        // Hash password (in production, use proper password hashing like BCrypt)
        user.PasswordHash = HashPassword(password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Create wallet for user
        var wallet = new Wallet
        {
            UserId = user.Id,
            Balance = 1000 // Starting balance
        };
        _context.Wallets.Add(wallet);
        await _context.SaveChangesAsync();

        return user;
    }

    /// <summary>
    /// Login user
    /// </summary>
    public async Task<User?> LoginAsync(string username, string password)
    {
        var user = await _context.Users
            .Include(u => u.Wallet)
            .Include(u => u.SkillProgress)
            .Include(u => u.Badges)
            .FirstOrDefaultAsync(u => u.Username == username);

        if (user == null)
            return null;

        // Verify password
        if (!VerifyPassword(password, user.PasswordHash))
            return null;

        return user;
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await _context.Users
            .Include(u => u.Wallet)
            .Include(u => u.SkillProgress)
            .Include(u => u.Badges)
            .Include(u => u.ReceivedRatings)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }

    /// <summary>
    /// Update user profile
    /// </summary>
    public async Task<bool> UpdateUserAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return true;
    }

    // Simple password hashing (use BCrypt in production)
    private string HashPassword(string password)
    {
        // This is a placeholder - use proper password hashing in production
        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
    }

    private bool VerifyPassword(string password, string hash)
    {
        var testHash = HashPassword(password);
        return testHash == hash;
    }
}
