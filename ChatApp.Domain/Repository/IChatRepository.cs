
using ChatApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Interfaces.Repository
{
    public interface IChatRepository
    {
        Task AddChatAsync(Chat chat);
        Task ArchivePrivateChat(Guid chatId, Guid userId, Guid contactId);
        Task<bool> GetChatStatusById(Guid ChatId, Guid ContactId);
        Task<Guid> GetChatIdAsync(Guid user1, Guid user2, CancellationToken token = default);
    }
}
