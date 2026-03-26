using ChatApp.Application.DTO;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ChatApp.Application.Interfaces.Service
{
    public interface IUserChatService
    {
        Task<List<UserChatDTO>> GetUserChatListAsync(Guid userId);
        Task<UserChatDTO?> GetUserChatDetailsAsync(Guid chatId, Guid userId, CancellationToken token);
        Task<bool> IsChatArchivedAsync(Guid chatId, Guid userId);
        Task ArchiveUserChatAsync(Guid chatId, Guid userId);
    }
}
