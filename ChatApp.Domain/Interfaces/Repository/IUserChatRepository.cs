using ChatApp.Domain.Entities;
using ChatApp.Domain.Models;

namespace ChatApp.Domain.Interfaces.Repository
{
    public interface IUserChatRepository
    {
        Task<bool> CheckIfGroupHaveAdminAsync(Guid chatId, Guid userId);
        Task UpdateAdminFlagAsync(Guid userId, Guid chatId, bool flag);
        Task UpdateLastReadMessageAsync(HashSet<Guid> userId, Guid chatId, Guid messageId);
        Task UpdateAliasOnChat(Guid userId, Guid chatId, string newAlias);
        Task<List<(Guid ChatId, int Count)>> CountAllUnreadMessageCountsAsync(Guid userId);
        Task<int> CountUnreadMessagesAsync(Guid userId, Guid chatId);
        Task ArchiveChatAsync(Guid chatId, Guid userId);
        Task ArchivePrivateChatAsync(Guid chatId, Guid userId, Guid contactId);
        Task UnarchiveChatAsync(Guid chatId, HashSet<Guid> userIds);
        Task MarkChatAsDeletedAsync(Guid chatId, Guid userId);
        Task SetChatAccessibilityAsync(Guid chatId, bool active, HashSet<Guid>? userIds = null);
        Task<bool> IsChatArchivedAsync(Guid chatId, Guid userId);
        Task<bool> GetChatStatusById(Guid chatId, Guid userId);
        Task<bool> GetUserAdminFlagAsync(Guid userId, Guid chatId);
        Task<bool> ExistsAsync(Guid chatId);
        Task<HashSet<Guid>> GetUsersInChatIdAsync(Guid chatId);
        Task<List<UserChat>?> GetAllUserChatsAsync(Guid userId);
        Task<UserChat?> GetUserChatAsync(Guid chatId, Guid userId, CancellationToken token);
        Task<Guid> GetReceiverUserIdAsync(Guid chatId, Guid userId, CancellationToken token);
        Task<string> GetPrivateUserAliasAsync(Guid chatId, Guid userId);
        Task<Dictionary<Guid, string>> GetPrivateUsersAliasesAsync(Guid userId, List<Guid> chatsIds);
        Task<Dictionary<Guid, string>> GetChatAliasesAsync(Guid chatId);
        Task<HashSet<ChatMemberInfo>> GetChatMembersAsync(Guid chatId);
        Task<DateTime?> GetLastReadMessageDateAsync(Guid chatId);
        Task<bool> HasMultipleMembersAsync(Guid chatId, Guid userId);
    }
}
