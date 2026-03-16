
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
        Task<Chat?> GetPrivateChatC(Guid user1, Guid user2);
        Task ArchivePrivateChat(Guid chatId, Guid userId, Guid contactId);
        Task RestoreChat(Guid chatId);
        Task<bool> GetChatStatusById(Guid ChatId, Guid ContactId);
        Task<List<UserChat>> GetChatListFromDb(Guid UserId);
    }
}
