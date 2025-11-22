using Microsoft.EntityFrameworkCore;
using SkillShareMap.Data;
using SkillShareMap.Models;
using System.Text;

namespace SkillShareMap.Services;

public class AIAssistantService : IAIAssistantService
{
    private readonly ApplicationDbContext _context;
    private readonly ITaskService _taskService;
    private readonly IAuthService _authService;

    public AIAssistantService(
        ApplicationDbContext context,
        ITaskService taskService,
        IAuthService authService)
    {
        _context = context;
        _taskService = taskService;
        _authService = authService;
    }

    /// <summary>
    /// Send a message to the AI assistant and get a response
    /// </summary>
    public async Task<string> SendMessageAsync(int userId, string message)
    {
        // Get or create conversation
        var conversation = await GetOrCreateConversationAsync(userId);

        // Add user message
        var userMessage = new AIMessage
        {
            ConversationId = conversation.Id,
            Content = message,
            Role = AIMessageRole.User,
            SentAt = DateTime.UtcNow
        };

        _context.AIMessages.Add(userMessage);

        // Generate AI response based on message content
        var response = await GenerateResponseAsync(userId, message, conversation);

        // Add AI response
        var aiMessage = new AIMessage
        {
            ConversationId = conversation.Id,
            Content = response,
            Role = AIMessageRole.Assistant,
            SentAt = DateTime.UtcNow
        };

        _context.AIMessages.Add(aiMessage);

        // Update conversation timestamp
        conversation.LastMessageAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return response;
    }

    /// <summary>
    /// Get conversation history for a user
    /// </summary>
    public async Task<AIConversation?> GetConversationAsync(int userId)
    {
        return await _context.AIConversations
            .Include(c => c.Messages.OrderBy(m => m.SentAt))
            .FirstOrDefaultAsync(c => c.UserId == userId);
    }

    /// <summary>
    /// Get AI recommendations for tasks based on user profile
    /// </summary>
    public async Task<List<SkillTask>> GetRecommendedTasksAsync(int userId)
    {
        var user = await _authService.GetUserByIdAsync(userId);
        if (user == null) return new List<SkillTask>();

        // Get all open tasks
        var tasks = await _taskService.GetTasksAsync(status: SkillTaskStatus.Open);

        // Get user's skill progress
        var userProgress = await _context.UserSkillProgress
            .Where(p => p.UserId == userId)
            .ToListAsync();

        // Filter and sort tasks based on user's skills and location
        var recommendedTasks = tasks
            .Where(t => t.Status == SkillTaskStatus.Open)
            .Where(t => userProgress.Any(p => p.Category == t.Category))
            .OrderByDescending(t => userProgress.FirstOrDefault(p => p.Category == t.Category)?.TotalXp ?? 0)
            .Take(10)
            .ToList();

        return recommendedTasks;
    }

    /// <summary>
    /// Get AI suggestions for skills to learn
    /// </summary>
    public async Task<List<string>> GetSkillSuggestionsAsync(int userId)
    {
        var user = await _authService.GetUserByIdAsync(userId);
        if (user == null) return new List<string>();

        // Get user's current skills
        var userProgress = await _context.UserSkillProgress
            .Where(p => p.UserId == userId)
            .ToListAsync();

        // Get all available categories
        var allCategories = Enum.GetValues<TaskCategory>().ToList();

        // Suggest categories user hasn't explored yet
        var suggestions = allCategories
            .Where(c => !userProgress.Any(p => p.Category == c))
            .Select(c => GetCategoryDescription(c))
            .ToList();

        return suggestions;
    }

    /// <summary>
    /// Clear conversation history for a user
    /// </summary>
    public async Task<bool> ClearConversationAsync(int userId)
    {
        var conversation = await _context.AIConversations
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (conversation == null) return false;

        _context.AIMessages.RemoveRange(conversation.Messages);
        await _context.SaveChangesAsync();

        return true;
    }

    // Private helper methods

    private async Task<AIConversation> GetOrCreateConversationAsync(int userId)
    {
        var conversation = await _context.AIConversations
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (conversation == null)
        {
            conversation = new AIConversation
            {
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                LastMessageAt = DateTime.UtcNow
            };

            _context.AIConversations.Add(conversation);
            await _context.SaveChangesAsync();
        }

        return conversation;
    }

    private async Task<string> GenerateResponseAsync(int userId, string message, AIConversation conversation)
    {
        var user = await _authService.GetUserByIdAsync(userId);
        if (user == null) return "I'm sorry, I couldn't find your user profile.";

        var lowerMessage = message.ToLower();

        // Task recommendations
        if (lowerMessage.Contains("recommend") || lowerMessage.Contains("task") || lowerMessage.Contains("find"))
        {
            var tasks = await GetRecommendedTasksAsync(userId);
            if (!tasks.Any())
            {
                return "I couldn't find any tasks that match your skills right now. Try exploring different categories or check back later!";
            }

            var sb = new StringBuilder();
            sb.AppendLine($"Based on your skills, I found {tasks.Count} tasks that might interest you:\n");

            foreach (var task in tasks.Take(5))
            {
                sb.AppendLine($"• {task.Title} - ${task.Budget}");
                sb.AppendLine($"  Category: {GetCategoryName(task.Category)}");
                sb.AppendLine($"  Location: {task.LocationAddress}\n");
            }

            sb.AppendLine("Would you like me to help you with anything else?");
            return sb.ToString();
        }

        // Skill suggestions
        if (lowerMessage.Contains("skill") || lowerMessage.Contains("learn") || lowerMessage.Contains("improve"))
        {
            var suggestions = await GetSkillSuggestionsAsync(userId);

            if (!suggestions.Any())
            {
                return "Great job! You've explored all available skill categories. Keep improving your existing skills!";
            }

            var sb = new StringBuilder();
            sb.AppendLine("Here are some new skills you might want to explore:\n");

            foreach (var suggestion in suggestions.Take(5))
            {
                sb.AppendLine($"• {suggestion}");
            }

            sb.AppendLine("\nTry accepting tasks in these categories to gain experience!");
            return sb.ToString();
        }

        // Wallet/earnings information
        if (lowerMessage.Contains("wallet") || lowerMessage.Contains("balance") || lowerMessage.Contains("money"))
        {
            var wallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (wallet == null)
            {
                return "I couldn't find your wallet information.";
            }

            return $"Your current wallet balance is ${wallet.Balance:F2}. You can earn more by completing tasks!";
        }

        // Profile/stats information
        if (lowerMessage.Contains("profile") || lowerMessage.Contains("stats") || lowerMessage.Contains("progress"))
        {
            var progress = await _context.UserSkillProgress
                .Where(p => p.UserId == userId)
                .ToListAsync();

            var completedTasks = await _context.SkillTasks
                .Where(t => t.AssignedToId == userId && t.Status == SkillTaskStatus.Completed)
                .CountAsync();

            var sb = new StringBuilder();
            sb.AppendLine($"Hello {user.Username}! Here's your profile summary:\n");
            sb.AppendLine($"• Completed Tasks: {completedTasks}");
            sb.AppendLine($"• Reputation: {user.ReputationLevel}/5 stars");
            sb.AppendLine($"• Skill Categories: {progress.Count}");

            if (progress.Any())
            {
                sb.AppendLine("\nYour skills:");
                foreach (var p in progress.OrderByDescending(p => p.TotalXp).Take(3))
                {
                    sb.AppendLine($"  • {GetCategoryName(p.Category)}: {p.TotalXp} XP");
                }
            }

            return sb.ToString();
        }

        // Help/greeting
        if (lowerMessage.Contains("help") || lowerMessage.Contains("hello") || lowerMessage.Contains("hi") || string.IsNullOrWhiteSpace(lowerMessage))
        {
            return $"Hello {user.Username}! I'm your AI assistant. I can help you with:\n\n" +
                   "• Finding recommended tasks based on your skills\n" +
                   "• Suggesting new skills to learn\n" +
                   "• Checking your wallet balance and earnings\n" +
                   "• Viewing your profile and progress\n\n" +
                   "Just ask me anything like 'recommend tasks' or 'what should I learn?'";
        }

        // Default response with context-aware suggestions
        var defaultResponses = new[]
        {
            $"I understand you're asking about '{message}'. I can help you find tasks, check your progress, or suggest skills to learn. What would you like to know more about?",
            $"That's interesting! I can help you with task recommendations, skill development, or checking your stats. What would you like to explore?",
            $"I'd be happy to help! Try asking me to 'recommend tasks' or 'suggest skills to learn'. What can I assist you with today?"
        };

        return defaultResponses[new Random().Next(defaultResponses.Length)];
    }

    private string GetCategoryName(TaskCategory category)
    {
        return category switch
        {
            TaskCategory.StudyHelp => "Study Help",
            TaskCategory.TechHelp => "Tech Help",
            TaskCategory.CreativeDesign => "Creative Design",
            TaskCategory.PhotoVideo => "Photo & Video",
            TaskCategory.WritingEditing => "Writing & Editing",
            TaskCategory.LanguageHelp => "Language Help",
            _ => category.ToString()
        };
    }

    private string GetCategoryDescription(TaskCategory category)
    {
        return category switch
        {
            TaskCategory.StudyHelp => "Study Help - Tutoring, homework, exam prep",
            TaskCategory.TechHelp => "Tech Help - Programming, IT support, troubleshooting",
            TaskCategory.CreativeDesign => "Creative Design - Graphic design, UI/UX, art",
            TaskCategory.PhotoVideo => "Photo & Video - Photography, videography, editing",
            TaskCategory.WritingEditing => "Writing & Editing - Content writing, proofreading",
            TaskCategory.LanguageHelp => "Language Help - Translation, language tutoring",
            _ => category.ToString()
        };
    }
}
