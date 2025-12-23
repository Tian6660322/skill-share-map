using SkillShareMap.Models;

namespace SkillShareMap.Services;

public interface ITaskService
{
    Task<SkillTask?> CreateTaskAsync(SkillTask task);
    Task<SkillTask?> GetTaskByIdAsync(int taskId);
    Task<List<SkillTask>> GetTasksAsync(TaskCategory? category = null, SkillTaskStatus? status = null);
    Task<List<SkillTask>> GetTasksNearLocationAsync(double latitude, double longitude, double radiusKm, TaskCategory? category = null);
    Task<List<TaskApplication>> GetTaskApplicationsAsync(int taskId);
    Task<bool> UpdateTaskAsync(SkillTask task);
    //Task<bool> AssignTaskAsync(int taskId, int userId); This could be changed as ADMIN Function?
    Task<bool> ApplyForTaskAsync(int taskId, int applicantId, decimal proposedPrice, string message);
    Task<bool> AcceptApplicationAsync(int applicationId, int creatorId);
    Task<bool> CompleteTaskAsync(int taskId);
    Task<bool> HelperMarkTaskDoneAsync(int taskId, int helperId);
    Task<bool> CreatorConfirmTaskDoneAsync(int taskId, int creatorId);
    Task<bool> CancelTaskAsync(int taskId);
    Task<bool> RepublishTaskAsync(int taskId);
    Task<bool> AcceptNegotiatedPriceAsync(int taskId, decimal newPrice);
}
