using SkillShareMap.Models;
namespace SkillShareMap.Services;

public interface IChatService
{
    Task<Message?> SendMessageAsync(Message message);
    Task<Message?> SendPriceNegotiationAsync(int senderId, int receiverId, int taskId, decimal proposedPrice);
    Task<bool> AcceptPriceNegotiationAsync(int messageId);
    Task<bool> RejectPriceNegotiationAsync(int messageId);
    Task<List<Message>> GetConversationAsync(int user1Id, int user2Id, int? taskId = null);
    Task<List<Message>> GetTaskMessagesAsync(int taskId);
    Task<List<Message>> GetUserMessagesAsync(int userId);
    Task<int> GetUnreadCountAsync(int userId);
    Task MarkAsReadAsync(int messageId);
}
