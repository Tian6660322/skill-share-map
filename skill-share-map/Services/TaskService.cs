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
            .FirstOrDefaultAsync(t => t.Id == taskId);
    }

    public async Task<List<SkillTask>> GetTasksByUserIdAsync(int userId)
    {
        return await _context.SkillTasks
                             .Where(task => task.CreatorId == userId)
                             .OrderByDescending(t => t.CreatedAt)
                             .ToListAsync();
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

        task.AssignedToId = userId;
        task.Status = SkillTaskStatus.PendingDeposit;
        await _context.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Accept negotiated price for a task
    /// </summary>
    public async Task<bool> AcceptNegotiatedPriceAsync(int taskId, decimal newPrice)
    {
        var task = await _context.SkillTasks.FindAsync(taskId);
        if (task == null)
            return false;

        task.NegotiatedPrice = newPrice;
        await _context.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Mark task as complete and process payment
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

}