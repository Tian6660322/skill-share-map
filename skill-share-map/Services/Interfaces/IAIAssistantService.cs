using SkillShareMap.Models;

namespace SkillShareMap.Services;

public interface IAIAssistantService
{
    /// <summary>
    /// Send a message to the AI assistant and get a response
    /// </summary>
    Task<string> SendMessageAsync(int userId, string message);

    /// <summary>
    /// Get conversation history for a user
    /// </summary>
    Task<AIConversation?> GetConversationAsync(int userId);

    /// <summary>
    /// Get AI recommendations for tasks based on user profile
    /// </summary>
    Task<List<SkillTask>> GetRecommendedTasksAsync(int userId);

    /// <summary>
    /// Get AI suggestions for skills to learn
    /// </summary>
    Task<List<string>> GetSkillSuggestionsAsync(int userId);

    /// <summary>
    /// Clear conversation history for a user
    /// </summary>
    Task<bool> ClearConversationAsync(int userId);
}
