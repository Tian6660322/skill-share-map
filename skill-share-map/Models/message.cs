namespace SkillShareMap.Models;

// This class stores chat messages between users
public class Message
{
    public int Id { get; set; }

    // Participants
    public int SenderId { get; set; }
    public User? Sender { get; set; }
    public int ReceiverId { get; set; }
    public User? Receiver { get; set; }

    // Related task (for task-specific chats)
    public int? TaskId { get; set; }
    public SkillTask? Task { get; set; }

    // Related job (for job inquiries)
    public int? JobId { get; set; }
    public Job? Job { get; set; }

    // Message content
    public string Content { get; set; } = string.Empty;

    // Message type (normal, price negotiation, system notification)
    public MessageType Type { get; set; } = MessageType.Normal;

    // For price negotiation messages
    public decimal? ProposedPrice { get; set; }
    public bool? IsAccepted { get; set; }

    // Timestamp and status
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public bool IsRead { get; set; } = false;
}

public enum MessageType
{
    Normal,              // Regular chat message
    PriceNegotiation,    // Price proposal from helper
    SystemNotification   // System-generated notification
}
