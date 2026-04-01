using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ChatApp.Application.Interfaces.Chats
{
    public interface IChatReadStatusService
    {
        Task MarkMessageAsReadAsync(Guid userId, Guid chatId, Guid messageId);
        Task MarkChatMessagesAsReadAsync(Guid userId, Guid chatId, CancellationToken token);
        Task<int> GetUnreadMessageCountAsync(Guid userId, Guid chatId);
        Task<List<(Guid ChatId, int Count)>> GetAllUnreadMessageCountsAsync(Guid userId);
        Task<DateTime?> GetLastMessageAtChatAsync(Guid userId, Guid chatId);
        Task SaveLastSentMessageIdAsync(Guid chatId, Guid messageId);
    }
}
