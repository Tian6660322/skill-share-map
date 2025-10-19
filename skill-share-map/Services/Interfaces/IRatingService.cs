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
