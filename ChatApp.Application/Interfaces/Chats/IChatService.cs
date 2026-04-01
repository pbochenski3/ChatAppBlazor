using ChatApp.Application.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatApp.Application.Interfaces.Chats
{
    public interface IChatService
    {
        Task<HashSet<Guid>> GetChatUsersIdsAsync(Guid chatId);
        Task DeleteChatAsync(Guid chatId, Guid userId);
        Task<bool> IsGroupChatExistingAsync(Guid chatId, Guid userId);
    }
}
