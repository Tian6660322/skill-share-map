using Microsoft.EntityFrameworkCore;
using SkillShareMap.Data;
using SkillShareMap.Models;

namespace SkillShareMap.Services;

public interface IReputationService
{
    Task<double> CalculateReputationLevelAsync(int userId);
    Task UpdateUserReputationAsync(int userId);
}

public class ReputationService : IReputationService
{
    private readonly ApplicationDbContext _context;

    public ReputationService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Calculate reputation level (1-5) based on average rating
    /// </summary>
    public async Task<double> CalculateReputationLevelAsync(int userId)
    {
        var ratings = await _context.Ratings
            .Where(r => r.ToUserId == userId)
            .ToListAsync();

        if (!ratings.Any())
            return 0;

        var averageRating = ratings.Average(r => r.Stars);

        // Round to nearest integer (1-5)
        return Math.Round(averageRating);
    }

    /// <summary>
    /// Update user's reputation level in the database
    /// </summary>
    public async Task UpdateUserReputationAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return;

        user.ReputationLevel = await CalculateReputationLevelAsync(userId);
        await _context.SaveChangesAsync();
    }
}
