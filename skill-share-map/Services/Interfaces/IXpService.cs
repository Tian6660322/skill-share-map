using SkillShareMap.Models;

namespace SkillShareMap.Services;

public interface IXpService
{
    int CalculateXpFromRating(int stars);
    Task AwardXpAsync(int userId, TaskCategory category, int xp);
    Task<BadgeTier> CheckAndAwardBadgeAsync(int userId, TaskCategory category);
    Task<UserSkillProgress?> GetSkillProgressAsync(int userId, TaskCategory category);
}
