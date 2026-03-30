using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatApp.Application.Interfaces.Service
{
    public interface IGroupChatService
    {
        Task<Guid> CreateGroupChatAsync(Guid existingChatId, HashSet<Guid> userIdsToAdd);
        Task AddUsersToGroupChatAsync(Guid chatId, HashSet<Guid> userIdsToAdd);
    }
}
