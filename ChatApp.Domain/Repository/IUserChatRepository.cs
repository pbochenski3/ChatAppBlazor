using ChatApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ChatApp.Domain.Repository
{
    public interface IUserChatRepository
    {
        Task UpdateLastReadMessageAsync(Guid userId, Guid chatId, Guid messageId);
        Task UpdateLastSentMessageAsync(Guid chatId, Guid messageId);
        Task<int> CountUnreadMessagesAsync(Guid userId, Guid chatId);
        Task<List<(Guid ChatId, int Count)>> CountAllUnreadMessageCountsAsync(Guid userId);
        Task<Guid> GetReceiverUserIdAsync(Guid chatId, Guid userId, CancellationToken token);
        Task<UserChat?> GetUserChatAsync(Guid chatId, Guid userId, CancellationToken token);
        Task<List<UserChat>?> GetAllUserChatsAsync(Guid userId);
        Task<HashSet<Guid>> GetUsersInChatAsync(Guid chatId);
        Task<bool> ExistsAsync(Guid chatId);
        Task ArchiveChatAsync(Guid chatId, Guid userId);
        Task<bool> IsChatArchivedAsync(Guid chatId, Guid userId);
        Task<DateTime?> GetLastReadAtAsync(Guid userId, Guid chatId);
        Task MarkChatAsDeletedAsync(Guid chatId, Guid userId);
        Task SetChatAccessibilityAsync(Guid chatId, bool active, HashSet<Guid>? userIds = null);
        Task UnarchiveChatAsync(Guid chatId, HashSet<Guid> userIds);
        Task ArchivePrivateChatAsync(Guid chatId, Guid userId, Guid contactId);
        Task<bool> GetChatStatusById(Guid chatId, Guid userId);
    }
}
