using Microsoft.EntityFrameworkCore;
using SkillShareMap.Data;
using SkillShareMap.Models;

namespace SkillShareMap.Services;

public class XpService : IXpService
{
    private readonly ApplicationDbContext _context;

    public XpService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Calculate XP based on star rating
    /// Formula: task_xp = round(base_points × rating_factor)
    /// Base points = 10
    /// </summary>
    public int CalculateXpFromRating(int stars)
    {
        const int basePoints = 10;

        var ratingFactor = stars switch
        {
            1 => 0.40,
            2 => 0.70,
            3 => 0.90,
            4 => 1.00,
            5 => 1.20,
            _ => 0
        };

        return (int)Math.Round(basePoints * ratingFactor);
    }

    /// <summary>
    /// Award XP to a user for a specific category
    /// </summary>
    public async Task AwardXpAsync(int userId, TaskCategory category, int xp)
    {
        // Find or create skill progress for this category
        var progress = await _context.UserSkillProgress
            .FirstOrDefaultAsync(p => p.UserId == userId && p.Category == category);

        if (progress == null)
        {
            progress = new UserSkillProgress
            {
                UserId = userId,
                Category = category,
                TotalXp = 0,
                CurrentTier = BadgeTier.Newbie
            };
            _context.UserSkillProgress.Add(progress);
        }

        // Add XP
        progress.TotalXp += xp;
        progress.LastUpdated = DateTime.UtcNow;

        // Update badge tier based on new XP total
        progress.CurrentTier = GetBadgeTierFromXp(progress.TotalXp);

        await _context.SaveChangesAsync();

        // Check if user earned a new badge
        await CheckAndAwardBadgeAsync(userId, category);
    }

    /// <summary>
    /// Get badge tier based on total XP
    /// </summary>
    private BadgeTier GetBadgeTierFromXp(int totalXp)
    {
        return totalXp switch
        {
            >= 1500 => BadgeTier.Master,
            >= 600 => BadgeTier.Expert,
            >= 200 => BadgeTier.Advanced,
            >= 50 => BadgeTier.Skilled,
            _ => BadgeTier.Newbie
        };
    }

    /// <summary>
    /// Check and award badge if user reached a new tier
    /// </summary>
    public async Task<BadgeTier> CheckAndAwardBadgeAsync(int userId, TaskCategory category)
    {
        var progress = await GetSkillProgressAsync(userId, category);
        if (progress == null)
            return BadgeTier.Newbie;

        var currentTier = progress.CurrentTier;

        // Check if user already has this badge
        var existingBadge = await _context.UserBadges
            .FirstOrDefaultAsync(b => b.UserId == userId &&
                                     b.Category == category &&
                                     b.Tier == currentTier);

        // Award badge if not already earned
        if (existingBadge == null)
        {
            var newBadge = new UserBadge
            {
                UserId = userId,
                Category = category,
                Tier = currentTier,
                EarnedAt = DateTime.UtcNow
            };

            _context.UserBadges.Add(newBadge);
            await _context.SaveChangesAsync();
        }

        return currentTier;
    }

    /// <summary>
    /// Get user's skill progress for a specific category
    /// </summary>
    public async Task<UserSkillProgress?> GetSkillProgressAsync(int userId, TaskCategory category)
    {
        return await _context.UserSkillProgress
            .FirstOrDefaultAsync(p => p.UserId == userId && p.Category == category);
    }
}
