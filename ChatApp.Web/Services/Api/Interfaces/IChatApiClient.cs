using ChatApp.Application.DTO;
using ChatApp.Application.DTO.Chats;

namespace ChatApp.Web.Services.Api.Interfaces
{
    public interface IChatApiClient
    {
        Task MarkMessageAsReadAsync(Guid chatId, Guid messageId);
        Task MarkAllMessagesAsReadAsync(Guid chatId, CancellationToken token);
        Task<UserChatDTO?> GetChatDetailsAsync(Guid chatId, CancellationToken token);
        Task<List<MessageDTO>> GetChatMessageHistoryAsync(Guid chatId, CancellationToken token);
        Task DeleteChatAsync(Guid chatId);
        Task ChangeChatNameAsync(Guid chatId, string chatName, string adminName,bool isGroup);
        Task<bool> IsChatExistingAsync(Guid chatId);
        Task<HashSet<Guid>> GetChatUsersIdsAsync(Guid chatId);
        Task ChangeUserAliasAsync(Guid chatId, Guid adminId, string newAlias, string adminName, Guid userId, string username);
        Task ChangeAdminFlagAsync(Guid chatId, Guid userId, bool flag);
        Task<bool> GetChatPermissions(Guid chatId, Guid userId);
    }
}
