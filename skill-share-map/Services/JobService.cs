using Microsoft.EntityFrameworkCore;
using SkillShareMap.Data;
using SkillShareMap.Models;

namespace SkillShareMap.Services;

public class JobService : IJobService
{
    private readonly ApplicationDbContext _context;

    public JobService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Create a new job posting
    /// </summary>
    public async Task<Job?> CreateJobAsync(Job job)
    {
        _context.Jobs.Add(job);
        await _context.SaveChangesAsync();
        return job;
    }

    /// <summary>
    /// Get job by ID
    /// </summary>
    public async Task<Job?> GetJobByIdAsync(int jobId)
    {
        return await _context.Jobs
            .Include(j => j.PostedBy)
            .FirstOrDefaultAsync(j => j.Id == jobId);
    }

    /// <summary>
    /// Get all open jobs
    /// </summary>
    public async Task<List<Job>> GetOpenJobsAsync()
    {
        return await _context.Jobs
            .Include(j => j.PostedBy)
            .Where(j => j.IsOpen)
            .OrderByDescending(j => j.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Get all jobs posted by a specific company
    /// </summary>
    public async Task<List<Job>> GetJobsByCompanyAsync(int companyId)
    {
        return await _context.Jobs
            .Include(j => j.PostedBy)
            .Where(j => j.PostedById == companyId)
            .OrderByDescending(j => j.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Update job details
    /// </summary>
    public async Task<bool> UpdateJobAsync(Job job)
    {
        _context.Jobs.Update(job);
        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Close a job posting
    /// </summary>
    public async Task<bool> CloseJobAsync(int jobId)
    {
        var job = await _context.Jobs.FindAsync(jobId);
        if (job == null)
            return false;

        job.IsOpen = false;
        job.ClosedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Delete a job posting
    /// </summary>
    public async Task<bool> DeleteJobAsync(int jobId)
    {
        var job = await _context.Jobs.FindAsync(jobId);
        if (job == null)
            return false;

        _context.Jobs.Remove(job);
        await _context.SaveChangesAsync();
        return true;
    }
}
