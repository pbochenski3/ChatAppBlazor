using ChatApp.Application.DTO;
using ChatApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Interfaces
{
    public interface IUserChatService
    {
        Task MarkMessageAsReadAsync(Guid userId, Guid chatId, Guid messageId);
        Task MarkChatMessagesAsReadAsync(Guid userId, Guid chatId, CancellationToken token);
        Task<int> GetUnreadCounterAsync(Guid userId, Guid chatId);
        Task SaveLastSendedChatMessageAsync(Guid chatId, Guid messageId);
        Task<Guid> GetReceiverUser(Guid chatId, Guid userId, CancellationToken token);
        Task<UserChatDTO> GetChatAsync(Guid chatId, Guid userId,CancellationToken token);
        Task<List<(Guid ChatId, int Count)>> GetAllUnreadCounterAsync(Guid userId);
        Task<List<UserChatDTO>> GetChatList(Guid userId);
        Task CreatePrivateChat(Guid user1, Guid user2);
        Task<Guid> GetChatId(Guid userId, Guid contactUserId, CancellationToken token);
        Task CreateGroupChat(Guid chatId, HashSet<Guid> UsersToAdd);
        Task<HashSet<Guid>> GetListOfUsersInChatAsync(Guid chatId);
        Task<bool> GetGroupChatByIdAsync(Guid chatId);
        Task AddUserGroupToDb(Guid chatId, HashSet<Guid> usersToAdd);
    }
}
