using SkillShareMap.Models;
namespace SkillShareMap.Services;

public interface IJobService
{
    Task<Job?> CreateJobAsync(Job job);
    Task<Job?> GetJobByIdAsync(int jobId);
    Task<List<Job>> GetOpenJobsAsync();
    Task<List<Job>> GetJobsByCompanyAsync(int companyId);
    Task<bool> UpdateJobAsync(Job job);
    Task<bool> CloseJobAsync(int jobId);
    Task<bool> DeleteJobAsync(int jobId);
}
