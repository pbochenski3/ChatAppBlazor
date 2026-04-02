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
        Task<Chat?> GetChatAsync(Guid userId1, Guid userId2, CancellationToken token = default);
        Task<bool> CheckIfGroupExist(Guid chatId, Guid userId);
        Task<Chat?> FetchChatById(Guid chatId);
        Task AddUserGroupToDb(Guid chatId, HashSet<Guid> userIdsToAdd);
        Task<HashSet<Guid>> GetExistingUsersInChat(Guid chatId, HashSet<Guid> userIdsToCheck);
        Task TryDeleteChatIfEmptyAsync(Guid chatId);
        Task UpdateChatNameAsync(Guid chatId, string chatName);
        Task UpdateGroupAvatarUrl(Guid chatId, string avatarUrl);
        Task<string> GetGroupAvatarUrlAsync(Guid chatId);
    }
}
