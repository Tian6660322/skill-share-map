using Microsoft.EntityFrameworkCore;
using SkillShareMap.Data;
using SkillShareMap.Models;

namespace SkillShareMap.Services;

public class TaskService : ITaskService
{
    private readonly ApplicationDbContext _context;
    private readonly IWalletService _walletService;

    public TaskService(ApplicationDbContext context, IWalletService walletService)
    {
        _context = context;
        _walletService = walletService;
    }

    /// <summary>
    /// Create a new task
    /// </summary>
    public async Task<SkillTask?> CreateTaskAsync(SkillTask task)
    {
        _context.SkillTasks.Add(task);
        await _context.SaveChangesAsync();
        return task;
    }

    /// <summary>
    /// Get task by ID with all related data
    /// </summary>
    public async Task<SkillTask?> GetTaskByIdAsync(int taskId)
    {
        return await _context.SkillTasks
            .Include(t => t.Creator)
            .Include(t => t.AssignedTo)
            .Include(t => t.Rating)
            .Include(t => t.Applications)
                .ThenInclude(a => a.Applicant)
            .FirstOrDefaultAsync(t => t.Id == taskId);
    }

    /// <summary>
    /// Get tasks with optional filters
    /// </summary>
    public async Task<List<SkillTask>> GetTasksAsync(TaskCategory? category = null, SkillTaskStatus? status = null)
    {
        var query = _context.SkillTasks
            .Include(t => t.Creator)
            .Include(t => t.AssignedTo)
            .AsQueryable();

        if (category.HasValue)
            query = query.Where(t => t.Category == category.Value);

        if (status.HasValue)
            query = query.Where(t => t.Status == status.Value);

        return await query.OrderByDescending(t => t.CreatedAt).ToListAsync();
    }

    /// <summary>
    /// Get tasks near a specific location within radius (in km)
    /// </summary>
    public async Task<List<SkillTask>> GetTasksNearLocationAsync(
        double latitude,
        double longitude,
        double radiusKm,
        TaskCategory? category = null)
    {
        // Get all tasks (we'll filter by distance in memory for simplicity)
        // In production, you'd use spatial queries
        var tasks = await GetTasksAsync(category);

        return tasks.Where(t =>
            t.Latitude.HasValue &&
            t.Longitude.HasValue &&
            CalculateDistance(latitude, longitude, t.Latitude.Value, t.Longitude.Value) <= radiusKm
        ).ToList();
    }

    /// <summary>
    /// Calculate distance between two coordinates using Haversine formula
    /// </summary>
    private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371; // Earth's radius in km

        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    private double ToRadians(double degrees) => degrees * Math.PI / 180;

    /// <summary>
    /// Update task details
    /// </summary>
    public async Task<bool> UpdateTaskAsync(SkillTask task)
    {
        _context.SkillTasks.Update(task);
        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Assign task to a user
    /// </summary>
    public async Task<bool> AssignTaskAsync(int taskId, int userId)
    {
        var task = await _context.SkillTasks.FindAsync(taskId);
        if (task == null || task.Status != SkillTaskStatus.Open)
            return false;

        // Check if user is a company or school - they cannot accept tasks
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return false;

        if (user.Role == UserRole.Company || user.Role == UserRole.School)
            return false; // Companies and schools can only post tasks, not accept them

        task.AssignedToId = userId;
        task.Status = SkillTaskStatus.PendingDeposit;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ApplyForTaskAsync(int taskId, int applicantId, decimal proposedPrice, string message)
    {
        // 1. check task status
        var task = await _context.SkillTasks
            .Include(t => t.Applications)
            .FirstOrDefaultAsync(t => t.Id == taskId);

        if (task == null || task.Status != SkillTaskStatus.Open)
            return false;

        // 2. applicant cannot be the creator
        if (task.CreatorId == applicantId)
            return false;

        // 3. check if already applied
        if (task.Applications.Any(a => a.ApplicantId == applicantId &&
            (a.Status == ApplicationStatus.Pending || a.Status == ApplicationStatus.Accepted)))
            return false;

        // 4. create application
        var application = new TaskApplication
        {
            TaskId = taskId,
            ApplicantId = applicantId,
            ProposedPrice = proposedPrice,
            Message = message,
            Status = ApplicationStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        _context.TaskApplications.Add(application);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AcceptApplicationAsync(int applicationId, int creatorId)
    {
        // 1. get application with task
        var application = await _context.TaskApplications
            .Include(a => a.Task)
            .FirstOrDefaultAsync(a => a.Id == applicationId);

        if (application == null || application.Task == null)
            return false;

        // 2. authenticate creator
        if (application.Task.CreatorId != creatorId)
            return false;

        // 3. validate task status
        if (application.Task.Status != SkillTaskStatus.Open)
            return false;

        application.Status = ApplicationStatus.Accepted;

        var otherApplications = await _context.TaskApplications
            .Where(a => a.TaskId == application.TaskId && a.Id != applicationId && a.Status == ApplicationStatus.Pending)
            .ToListAsync();

        foreach (var otherApp in otherApplications)
        {
            otherApp.Status = ApplicationStatus.Rejected;
        }

        var task = application.Task;
        task.AssignedToId = application.ApplicantId;
        task.NegotiatedPrice = application.ProposedPrice;

        task.Status = SkillTaskStatus.PendingDeposit;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<TaskApplication>> GetTaskApplicationsAsync(int taskId)
    {
        return await _context.TaskApplications
            .Include(a => a.Applicant)
            .Where(a => a.TaskId == taskId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Mark task as complete and process payment (DEPRECATED - use two-step confirmation instead)
    /// </summary>
    public async Task<bool> CompleteTaskAsync(int taskId)
    {
        var task = await _context.SkillTasks.FindAsync(taskId);
        if (task == null || task.Status != SkillTaskStatus.Assigned)
            return false;

        task.Status = SkillTaskStatus.Completed;
        task.CompletedAt = DateTime.UtcNow;
        task.IsCompleted = true;

        await _context.SaveChangesAsync();

        // Process payment to helper
        await _walletService.ProcessPaymentAsync(taskId);

        return true;
    }

    /// <summary>
    /// Helper marks task as done (first step of completion)
    /// </summary>
    public async Task<bool> HelperMarkTaskDoneAsync(int taskId, int helperId)
    {
        var task = await _context.SkillTasks.FindAsync(taskId);
        if (task == null || task.Status != SkillTaskStatus.Assigned || task.AssignedToId != helperId)
            return false;

        task.IsHelperConfirmedComplete = true;
        task.HelperConfirmedAt = DateTime.UtcNow;
        task.Status = SkillTaskStatus.PendingCompletion;

        await _context.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Creator confirms task is done (second step - triggers payment)
    /// </summary>
    public async Task<bool> CreatorConfirmTaskDoneAsync(int taskId, int creatorId)
    {
        var task = await _context.SkillTasks.FindAsync(taskId);
        if (task == null || task.Status != SkillTaskStatus.PendingCompletion || task.CreatorId != creatorId)
            return false;

        task.IsCreatorConfirmedComplete = true;
        task.CreatorConfirmedAt = DateTime.UtcNow;
        task.Status = SkillTaskStatus.Completed;
        task.CompletedAt = DateTime.UtcNow;
        task.IsCompleted = true;

        await _context.SaveChangesAsync();

        // Process payment to helper
        await _walletService.ProcessPaymentAsync(taskId);

        return true;
    }

    /// <summary>
    /// Cancel task and refund deposit if paid
    /// </summary>
    public async Task<bool> CancelTaskAsync(int taskId)
    {
        var task = await _context.SkillTasks.FindAsync(taskId);
        if (task == null)
            return false;

        task.Status = SkillTaskStatus.Cancelled;
        await _context.SaveChangesAsync();

        // Refund deposit if it was paid
        if (task.IsDepositPaid)
        {
            await _walletService.ProcessRefundAsync(taskId);
        }

        return true;
    }

    /// <summary>
    /// Republish a cancelled task (make it Open again)
    /// </summary>
    public async Task<bool> RepublishTaskAsync(int taskId)
    {
        var task = await _context.SkillTasks.FindAsync(taskId);
        if (task == null || task.Status != SkillTaskStatus.Cancelled)
            return false;

        // Reset task to Open status
        task.Status = SkillTaskStatus.Open;
        task.AssignedToId = null;
        task.IsDepositPaid = false;
        task.DepositAmount = 0;
        task.NegotiatedPrice = null;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> AcceptNegotiatedPriceAsync(int taskId, decimal newPrice)
    {
        var task = await _context.SkillTasks.FindAsync(taskId);
        if (task == null) return false;

        task.NegotiatedPrice = newPrice;
        await _context.SaveChangesAsync();
        return true;
    }
}