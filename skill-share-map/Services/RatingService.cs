using Microsoft.EntityFrameworkCore;
using SkillShareMap.Data;
using SkillShareMap.Models;

namespace SkillShareMap.Services;

public interface IRatingService
{
    Task<Rating?> CreateRatingAsync(Rating rating);
    Task<List<Rating>> GetReceivedRatingsAsync(int userId);
    Task<List<Rating>> GetGivenRatingsAsync(int userId);
    Task<double> GetAverageRatingAsync(int userId);
    Task<List<Rating>> GetRatingsForUserAsync(int userId);
    Task<List<UserBadge>> GetUserBadgesAsync(int userId);
}

public class RatingService : IRatingService
{
    private readonly ApplicationDbContext _context;
    private readonly IXpService _xpService;
    private readonly IReputationService _reputationService;

    public RatingService(
        ApplicationDbContext context,
        IXpService xpService,
        IReputationService reputationService)
    {
        _context = context;
        _xpService = xpService;
        _reputationService = reputationService;
    }

    /// <summary>
    /// Create a new rating and update XP/reputation accordingly
    /// </summary>
    public async Task<Rating?> CreateRatingAsync(Rating rating)
    {
        // Validate rating
        if (rating.Stars < 1 || rating.Stars > 5)
            throw new ArgumentException("Rating must be between 1 and 5 stars");

        if (string.IsNullOrWhiteSpace(rating.Comment))
            throw new ArgumentException("Comment is required for rating");

        // Calculate XP from rating
        var xp = _xpService.CalculateXpFromRating(rating.Stars);
        rating.XpAwarded = xp;

        // Add rating to database
        _context.Ratings.Add(rating);
        await _context.SaveChangesAsync();

        // Get the user who received the rating
        var toUser = await _context.Users.FindAsync(rating.ToUserId);
        if (toUser == null)
            return rating;

        // Award XP if the recipient is a helper (assigned to task) and has a category
        if (rating.Category.HasValue)
        {
            var task = await _context.SkillTasks
                .FirstOrDefaultAsync(t => t.Id == rating.TaskId);

            // Only award XP to the person who completed the task (helper)
            if (task != null && task.AssignedToId == rating.ToUserId)
            {
                await _xpService.AwardXpAsync(
                    rating.ToUserId,
                    rating.Category.Value,
                    xp);
            }
        }

        // Update reputation for both users (giver and receiver)
        await _reputationService.UpdateUserReputationAsync(rating.ToUserId);
        await _reputationService.UpdateUserReputationAsync(rating.FromUserId);

        return rating;
    }

    /// <summary>
    /// Get all ratings received by a user
    /// </summary>
    public async Task<List<Rating>> GetReceivedRatingsAsync(int userId)
    {
        return await _context.Ratings
            .Include(r => r.FromUser)
            .Include(r => r.Task)
            .Where(r => r.ToUserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Get all ratings given by a user
    /// </summary>
    public async Task<List<Rating>> GetGivenRatingsAsync(int userId)
    {
        return await _context.Ratings
            .Include(r => r.ToUser)
            .Include(r => r.Task)
            .Where(r => r.FromUserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Get average rating for a user
    /// </summary>
    public async Task<double> GetAverageRatingAsync(int userId)
    {
        var ratings = await _context.Ratings
            .Where(r => r.ToUserId == userId)
            .ToListAsync();

        if (!ratings.Any())
            return 0;

        return ratings.Average(r => r.Stars);
    }

    /// <summary>
    /// Get ratings for a user (same as GetReceivedRatingsAsync)
    /// </summary>
    public async Task<List<Rating>> GetRatingsForUserAsync(int userId)
    {
        return await GetReceivedRatingsAsync(userId);
    }

    /// <summary>
    /// Get all badges for a user
    /// </summary>
    public async Task<List<UserBadge>> GetUserBadgesAsync(int userId)
    {
        return await _context.UserBadges
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.Tier)
            .ToListAsync();
    }
}
