namespace ChatApp.Application.Interfaces.Chats
{
    public interface IGroupChatService
    {
        //Task<Guid> CreateGroupChatAsync(Guid existingChatId, HashSet<Guid> userIdsToAdd);
        Task AddUsersToGroupChatAsync(Guid chatId, HashSet<Guid> userIdsToAdd);
        //Task ProcessAddToGroupChatAsync(Guid chatId, HashSet<Guid> usersToAdd, Guid userId);
        Task ProcessLeaveGroupChatAsync(Guid chatId, Guid userId, string username);
        //Task<HashSet<UserDTO>> ProccesGetChatUsersAsync(Guid chatId);
    }
}
