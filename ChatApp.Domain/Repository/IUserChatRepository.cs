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
        Task MarkChatAsReadAsync(Guid userId, Guid chatId, CancellationToken token);
        Task<Guid> GetReceiverUserIdAsync(Guid chatId, Guid userId, CancellationToken token);
        Task<UserChat?> GetUserChatAsync(Guid chatId, Guid userId, CancellationToken token);
        Task<List<UserChat>?> GetAllUserChatsAsync(Guid userId);
        Task RestoreChatAsync(Guid chatId);
        Task<HashSet<Guid>> GetUsersInChatAsync(Guid chatId);
        Task<bool> ExistsAsync(Guid chatId);
        Task ArchiveChatAsync(Guid chatId, Guid userId);
        Task RestoreGroupChatForUsersAsync(Guid chatId, HashSet<Guid> userIds);
        Task<bool> IsChatArchivedAsync(Guid chatId, Guid userId);
        Task<DateTime?> GetLastReadAtAsync(Guid userId, Guid chatId);
        Task MarkChatAsDeletedAsync(Guid chatId, Guid userId);
    }
}
