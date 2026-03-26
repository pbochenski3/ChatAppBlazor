
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
        Task<bool> CheckIfGroupExist(Guid chatId, Guid userId);
        Task<Chat> FetchChatById(Guid chatId);
        Task AddUserGroupToDb(Guid chatId, HashSet<Guid> usersToAdd);
        Task<HashSet<Guid>> GetExistingUsersInChat(Guid chatId, HashSet<Guid> usersToCheck);
        Task UnArchiveChat(Guid chatId, HashSet<Guid> usersId);
        Task TryDeleteChatIfEmptyAsync(Guid chatId);

    }
}
