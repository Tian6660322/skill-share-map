using SkillShareMap.Models;
namespace SkillShareMap.Services;

public interface ICourseService
{
    Task<List<Course>> GetAllCoursesAsync();
    Task<List<Course>> GetCoursesByCategoryAsync(TaskCategory category);
    Task<Course?> GetCourseByIdAsync(int courseId);
}
