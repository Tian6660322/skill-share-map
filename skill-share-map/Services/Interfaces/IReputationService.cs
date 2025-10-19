using SkillShareMap.Models;

namespace SkillShareMap.Services;

public interface IReputationService
{
    Task<double> CalculateReputationLevelAsync(int userId);
    Task UpdateUserReputationAsync(int userId);
}
