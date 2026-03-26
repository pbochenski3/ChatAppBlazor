using ChatApp.Application.DTO;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ChatApp.Application.Interfaces.Service
{
    public interface IChatService
    {
        Task<bool> IsChatArchivedAsync(Guid chatId, Guid userId);
        Task<bool> IsGroupChatExistingAsync(Guid chatId, Guid userId);
        Task<ChatDTO> GetChatDetailsAsync(Guid chatId);
        Task MarkMessageAsReadAsync(Guid userId, Guid chatId, Guid messageId);
        Task<UserChatDTO?> GetUserChatDetailsAsync(Guid chatId, Guid userId, CancellationToken token);
        Task MarkChatMessagesAsReadAsync(Guid userId, Guid chatId, CancellationToken token);
        Task SaveLastSentMessageIdAsync(Guid chatId, Guid messageId);
        Task<int> GetUnreadMessageCountAsync(Guid userId, Guid chatId);
        Task<List<(Guid ChatId, int Count)>> GetAllUnreadMessageCountsAsync(Guid userId);
        Task<Guid> GetReceiverUserIdAsync(Guid chatId, Guid userId, CancellationToken token);
        Task<Guid> GetPrivateChatIdAsync(Guid userId, Guid contactUserId, CancellationToken token);
        Task<HashSet<Guid>> GetChatUsersIdsAsync(Guid chatId);
        Task<Guid> CreateGroupChatAsync(Guid existingChatId, HashSet<Guid> userIdsToAdd);
        Task CreatePrivateChatAsync(Guid userId1, Guid userId2);
        Task<List<UserChatDTO>> GetUserChatListAsync(Guid userId);
        Task ArchiveUserChatAsync(Guid chatId, Guid userId);
        Task AddUsersToGroupChatAsync(Guid chatId, HashSet<Guid> userIdsToAdd);
        Task<DateTime?> GetLastSeenMessageAtAsync(Guid userId, Guid chatId);
        Task DeleteChatAsync(Guid chatId, Guid userId);
    }
}
