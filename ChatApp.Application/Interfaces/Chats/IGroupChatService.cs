using ChatApp.Application.DTO;
using ChatApp.Application.DTO.Results;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatApp.Application.Interfaces.Chats
{
    public interface IGroupChatService
    {
        Task<Guid> CreateGroupChatAsync(Guid existingChatId, HashSet<Guid> userIdsToAdd);
        Task AddUsersToGroupChatAsync(Guid chatId, HashSet<Guid> userIdsToAdd);
        Task<AddToGroupActionResult> ProcessAddToGroupChatAsync(Guid chatId, HashSet<Guid> usersToAdd, Guid userId);
        Task<MessageDTO> ProcessLeaveGroupChatAsync(Guid chatId, Guid userId, string username);
        Task<HashSet<UserDTO>> ProccesGetChatUsersAsync(Guid chatId);
    }
}
