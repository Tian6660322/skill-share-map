using Microsoft.EntityFrameworkCore;
using SkillShareMap.Data;
using SkillShareMap.Models;
using System.Net.Mail;

namespace SkillShareMap.Services;

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
        // 1. verify if username or email already exists
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == user.Username || u.Email == user.Email); // 👈 修正点

        if (existingUser != null)
            return null;

        // Hash password 
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
    /// Login user (using username OR email as identifier)
    /// </summary>
    public async Task<User?> LoginAsync(string identifier, string password)
    {
        // 1. verify if identifier is email
        bool isValidEmail = false;
        try
        {
            var addr = new MailAddress(identifier);
            isValidEmail = (addr.Address == identifier);
        }
        catch
        {
            isValidEmail = false;
        }

        IQueryable<User> query = _context.Users
            .Include(u => u.Wallet)
            .Include(u => u.SkillProgress)
            .Include(u => u.Badges);

        // 2. search by email or username
        if (isValidEmail)
        {
            // if yes, search by email
            query = query.Where(u => u.Email == identifier);
        }
        else
        {
            // if not, search by username
            query = query.Where(u => u.Username == identifier);
        }

        // 3. search for user
        var user = await query.FirstOrDefaultAsync();

        if (user == null)
            return null;

        // 4. verify password
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

    /// <summary>
    /// Check if a username or email already exists.
    /// </summary>
    public async Task<bool> CheckIdentifierExistsAsync(string identifier)
    {
        bool isEmail = false;
        try
        {
            new System.Net.Mail.MailAddress(identifier);
            isEmail = true;
        }
        catch { /* not a valid email */ }

        if (isEmail)
        {
            return await _context.Users.AnyAsync(u => u.Email == identifier);
        }
        else
        {
            return await _context.Users.AnyAsync(u => u.Username == identifier);
        }
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
