using Microsoft.EntityFrameworkCore;
using SkillShareMap.Data;
using SkillShareMap.Models;

namespace SkillShareMap.Services;

public class ChatService : IChatService
{
    private readonly ApplicationDbContext _context;
    private readonly ITaskService _taskService;

    public ChatService(ApplicationDbContext context, ITaskService taskService)
    {
        _context = context;
        _taskService = taskService;
    }

    /// <summary>
    /// Send a regular message
    /// </summary>
    public async Task<Message?> SendMessageAsync(Message message)
    {
        message.SentAt = DateTime.UtcNow;
        message.IsRead = false;

        _context.Messages.Add(message);
        await _context.SaveChangesAsync();
        return message;
    }

    /// <summary>
    /// Send a price negotiation message
    /// </summary>
    public async Task<Message?> SendPriceNegotiationAsync(
        int senderId,
        int receiverId,
        int taskId,
        decimal proposedPrice)
    {
        var message = new Message
        {
            SenderId = senderId,
            ReceiverId = receiverId,
            TaskId = taskId,
            Type = MessageType.PriceNegotiation,
            Content = $"I would like to negotiate the price to ${proposedPrice}",
            ProposedPrice = proposedPrice,
            IsAccepted = null,
            SentAt = DateTime.UtcNow,
            IsRead = false
        };

        _context.Messages.Add(message);
        await _context.SaveChangesAsync();
        return message;
    }

    /// <summary>
    /// Accept a price negotiation
    /// </summary>
    public async Task<bool> AcceptPriceNegotiationAsync(int messageId)
    {
        var message = await _context.Messages.FindAsync(messageId);
        if (message == null ||
            message.Type != MessageType.PriceNegotiation ||
            !message.ProposedPrice.HasValue ||
            !message.TaskId.HasValue)
            return false;

        // Mark message as accepted
        message.IsAccepted = true;
        await _context.SaveChangesAsync();

        // Update task with negotiated price
        await _taskService.AcceptNegotiatedPriceAsync(message.TaskId.Value, message.ProposedPrice.Value);

        // Send confirmation message
        var confirmMessage = new Message
        {
            SenderId = message.ReceiverId,
            ReceiverId = message.SenderId,
            TaskId = message.TaskId,
            Type = MessageType.SystemNotification,
            Content = $"Price negotiation accepted! New price: ${message.ProposedPrice.Value}",
            SentAt = DateTime.UtcNow
        };
        _context.Messages.Add(confirmMessage);
        await _context.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Reject a price negotiation
    /// </summary>
    public async Task<bool> RejectPriceNegotiationAsync(int messageId)
    {
        var message = await _context.Messages.FindAsync(messageId);
        if (message == null || message.Type != MessageType.PriceNegotiation)
            return false;

        // Mark message as rejected
        message.IsAccepted = false;
        await _context.SaveChangesAsync();

        // Send rejection message
        var rejectMessage = new Message
        {
            SenderId = message.ReceiverId,
            ReceiverId = message.SenderId,
            TaskId = message.TaskId,
            Type = MessageType.SystemNotification,
            Content = "Price negotiation was rejected.",
            SentAt = DateTime.UtcNow
        };
        _context.Messages.Add(rejectMessage);
        await _context.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Get conversation between two users
    /// </summary>
    public async Task<List<Message>> GetConversationAsync(int user1Id, int user2Id, int? taskId = null)
    {
        var query = _context.Messages
            .Include(m => m.Sender)
            .Include(m => m.Receiver)
            .Where(m =>
                (m.SenderId == user1Id && m.ReceiverId == user2Id) ||
                (m.SenderId == user2Id && m.ReceiverId == user1Id));

        if (taskId.HasValue)
            query = query.Where(m => m.TaskId == taskId.Value);

        return await query.OrderBy(m => m.SentAt).ToListAsync();
    }

    /// <summary>
    /// Get all messages for a specific task
    /// </summary>
    public async Task<List<Message>> GetTaskMessagesAsync(int taskId)
    {
        return await _context.Messages
            .Include(m => m.Sender)
            .Include(m => m.Receiver)
            .Where(m => m.TaskId == taskId)
            .OrderBy(m => m.SentAt)
            .ToListAsync();
    }

    /// <summary>
    /// Get all messages for a user (sent or received)
    /// </summary>
    public async Task<List<Message>> GetUserMessagesAsync(int userId)
    {
        return await _context.Messages
            .Include(m => m.Sender)
            .Include(m => m.Receiver)
            .Where(m => m.SenderId == userId || m.ReceiverId == userId)
            .OrderBy(m => m.SentAt)
            .ToListAsync();
    }

    /// <summary>
    /// Get count of unread messages for a user
    /// </summary>
    public async Task<int> GetUnreadCountAsync(int userId)
    {
        return await _context.Messages
            .Where(m => m.ReceiverId == userId && !m.IsRead)
            .CountAsync();
    }

    /// <summary>
    /// Mark a message as read
    /// </summary>
    public async Task MarkAsReadAsync(int messageId)
    {
        var message = await _context.Messages.FindAsync(messageId);
        if (message != null)
        {
            message.IsRead = true;
            await _context.SaveChangesAsync();
        }
    }
}
