using ChatApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Domain.Repository
{
    public interface IUserChatRepository
    {
        Task SaveLastReadMessage(Guid userId, Guid chatId, Guid messageId);
        Task SaveLastSendedChatMessage(Guid chatId, Guid messageId);
        Task<int> CountUnreadMessagesAsync(Guid userId, Guid chatId);
        Task<List<(Guid ChatId, int Count)>> CountAllUnreadMessagesAsync(Guid userId);
        Task SaveChatAsReaded(Guid userId, Guid chatId,CancellationToken token);
        Task<Guid> FetchReceiverUser(Guid chatId, Guid userId, CancellationToken token);
        Task<UserChat?> FetchChatAsync(Guid chatId, Guid userId, CancellationToken token);
        Task<List<UserChat>?> FetchAllChatsAsync(Guid userId);
        Task<bool> CheckIfChatExisted(Guid chatId);
        Task RestoreChat(Guid chatId);
    }
}
