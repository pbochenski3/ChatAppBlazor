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
        Task<List<CounterBadge>> CountAllUnreadMessagesAsync(Guid userId);
    }
}
