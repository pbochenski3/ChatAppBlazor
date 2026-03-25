using ChatApp.Application.DTO;
using ChatApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Interfaces.Service
{
    public interface IChatService
    {
        Task<bool> GetChatStatus(Guid ChatId, Guid ContactId);
        Task<bool> GetGroupChatByIdAsync(Guid chatId, Guid userId);
        Task<ChatDTO> GetChatById(Guid chatId);
        Task MarkMessageAsReadAsync(Guid userId, Guid chatId, Guid messageId);
        Task<UserChatDTO> GetChatAsync(Guid chatId, Guid userId, CancellationToken token);
        Task MarkChatMessagesAsReadAsync(Guid userId, Guid chatId, CancellationToken token);
        Task SaveLastSendedChatMessageAsync(Guid chatId, Guid messageId);
        Task<int> GetUnreadCounterAsync(Guid userId, Guid chatId);
        Task<List<(Guid ChatId, int Count)>> GetAllUnreadCounterAsync(Guid userId);
        Task<Guid> GetReceiverUser(Guid chatId, Guid userId, CancellationToken token);
        Task<Guid> GetChatId(Guid userId, Guid contactUserId, CancellationToken token);
        Task<HashSet<Guid>> GetListOfUsersInChatAsync(Guid chatId);
        Task CreateGroupChat(Guid chatId, HashSet<Guid> UsersToAdd);
        Task CreatePrivateChat(Guid user1, Guid user2);
        Task<List<UserChatDTO>> GetChatList(Guid userId);
        Task ArchiveUserGroupChat(Guid chatId, Guid userId);
        Task AddUsersToGroup(Guid chatId, HashSet<Guid> usersToAdd);

    }
}
