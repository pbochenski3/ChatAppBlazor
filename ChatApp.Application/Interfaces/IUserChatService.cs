using ChatApp.Application.DTO;
using ChatApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Interfaces
{
    public interface IUserChatService
    {
        Task MarkChatAsReadAsync(Guid userId, Guid chatId, Guid messageId);
        Task SaveLastSendedChatMessage(Guid chatId, Guid messageId);
        Task<int> GetUnreadCounterAsync(Guid userId, Guid chatId);
        Task<List<CounterBadge>> GetAllUnreadCounterAsync(Guid userId);
        Task SaveLastMessageAsync(Guid chatId, Guid messageId);
    }
}
