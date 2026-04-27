using ChatApp.Domain.Models;

namespace ChatApp.Domain.Interfaces.Repository
{
    public interface IUserChatRepository
    {
        #region Read & Sync Status
        Task UpdateLastReadMessageAsync(Guid userId, Guid chatId, Guid messageId);
        Task UpdateLastSentMessageAsync(Guid chatId, Guid messageId);
        Task<List<(Guid ChatId, int Count)>> CountAllUnreadMessageCountsAsync(Guid userId);
        Task<int> CountUnreadMessagesAsync(Guid userId, Guid chatId);
        #endregion
        #region Visibility & Archive
        Task ArchiveChatAsync(Guid chatId, Guid userId);
        Task ArchivePrivateChatAsync(Guid chatId, Guid userId, Guid contactId);
        Task UnarchiveChatAsync(Guid chatId, HashSet<Guid> userIds);
        Task MarkChatAsDeletedAsync(Guid chatId, Guid userId);
        Task SetChatAccessibilityAsync(Guid chatId, bool active, HashSet<Guid>? userIds = null);
        Task<bool> IsChatArchivedAsync(Guid chatId, Guid userId);
        Task<bool> GetChatStatusById(Guid chatId, Guid userId);
        Task SetNewChatNameAsync(Guid chatId, Guid userId, string newName);
        #endregion
        #region Queries & Membership
        Task<DateTime?> GetLastMessageDateAsync(Guid userId, Guid chatId);
        Task<bool> ExistsAsync(Guid chatId);
        Task<HashSet<Guid>> GetUsersInChatIdAsync(Guid chatId);
        Task<List<UserChat>?> GetAllUserChatsAsync(Guid userId);
        Task<UserChat?> GetUserChatAsync(Guid chatId, Guid userId, CancellationToken token);
        Task<Guid> GetReceiverUserIdAsync(Guid chatId, Guid userId, CancellationToken token);
        #endregion
    }
}
