namespace SkillShareMap.Models;

/// <summary>
/// AI conversation history for each user
/// </summary>
public class AIConversation
{
    public int Id { get; set; }

    // User who owns this conversation
    public int UserId { get; set; }
    public User? User { get; set; }

    // Conversation messages
    public List<AIMessage> Messages { get; set; } = new();

    // Metadata
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastMessageAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Individual AI message in a conversation
/// </summary>
public class AIMessage
{
    public int Id { get; set; }

    // Parent conversation
    public int ConversationId { get; set; }
    public AIConversation? Conversation { get; set; }

    // Message content
    public string Content { get; set; } = string.Empty;
    public AIMessageRole Role { get; set; }

    // Timestamp
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Role of the message sender
/// </summary>
public enum AIMessageRole
{
    User,       // Message from the user
    Assistant   // Response from AI assistant
}
