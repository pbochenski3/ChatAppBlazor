
using ChatApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Interfaces.Repository
{
    public interface IChatRepository
    {
        Task AddChatAsync(Chat chat);
        Task SaveChangesToDbAsync();
        Task<Chat?> GetPrivateChat(Guid user1, Guid user2);
        Task ArchivePrivateChatFromDb(Guid chatId, Guid userId, Guid contactId);
        Task RestoreChat(Guid chatId);
    }
}
