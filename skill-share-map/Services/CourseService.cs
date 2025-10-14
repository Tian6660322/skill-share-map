using Microsoft.EntityFrameworkCore;
using SkillShareMap.Data;
using SkillShareMap.Models;

namespace SkillShareMap.Services;

public interface ICourseService
{
    Task<List<Course>> GetAllCoursesAsync();
    Task<List<Course>> GetCoursesByCategoryAsync(TaskCategory category);
    Task<Course?> GetCourseByIdAsync(int courseId);
}

public class CourseService : ICourseService
{
    private readonly ApplicationDbContext _context;

    public CourseService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get all courses
    /// </summary>
    public async Task<List<Course>> GetAllCoursesAsync()
    {
        return await _context.Courses
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Get courses by category
    /// </summary>
    public async Task<List<Course>> GetCoursesByCategoryAsync(TaskCategory category)
    {
        return await _context.Courses
            .Where(c => c.Category == category)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Get course by ID
    /// </summary>
    public async Task<Course?> GetCourseByIdAsync(int courseId)
    {
        return await _context.Courses.FindAsync(courseId);
    }
}
