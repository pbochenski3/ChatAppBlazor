using ChatApp.Application.DTO;
using ChatApp.Application.DTO.Chats;

namespace ChatApp.ChatServer.Client.Services.Interfaces
{
    public interface IChatApiClient
    {
        Task MarkMessageAsReadAsync(Guid chatId, Guid messageId);
        Task MarkAllMessagesAsReadAsync(Guid chatId, CancellationToken token);
        Task<UserChatDTO?> GetChatDetailsAsync(Guid chatId, CancellationToken token);
        Task<List<MessageDTO>> GetChatMessageHistoryAsync(Guid chatId, CancellationToken token);
        Task DeleteChatAsync(Guid chatId);
        Task ChangeChatNameAsync(Guid chatId, string chatName,string adminName);
    }
}
