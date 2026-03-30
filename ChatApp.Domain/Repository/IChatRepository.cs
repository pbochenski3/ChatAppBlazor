using ChatApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ChatApp.Application.Interfaces.Repository
{
    public interface IChatRepository
    {
        Task AddChatAsync(Chat chat);
        Task ArchivePrivateChatAsync(Guid chatId, Guid userId, Guid contactId);
        Task<bool> GetChatStatusById(Guid chatId, Guid contactId);
        Task<Chat?> GetChatAsync(Guid userId1, Guid userId2, CancellationToken token = default);
        Task<bool> CheckIfGroupExist(Guid chatId, Guid userId);
        Task<Chat?> FetchChatById(Guid chatId);
        Task AddUserGroupToDb(Guid chatId, HashSet<Guid> userIdsToAdd);
        Task<HashSet<Guid>> GetExistingUsersInChat(Guid chatId, HashSet<Guid> userIdsToCheck);
        Task UnarchiveChatAsync(Guid chatId, HashSet<Guid> userIds);
        Task TryDeleteChatIfEmptyAsync(Guid chatId);
    }
}
