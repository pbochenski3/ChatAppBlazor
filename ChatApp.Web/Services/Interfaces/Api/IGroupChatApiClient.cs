using ChatApp.Application.DTO;

namespace ChatApp.Web.Services.Interfaces.Api
{
    public interface IGroupChatApiClient
    {

        Task CreateGroupChatAsync(Guid chatId, HashSet<Guid> userIdsToAdd);
        Task AddUsersToGroupChatAsync(Guid chatId, HashSet<Guid> userIdsToAdd);
        Task<bool> LeaveGroupChatAsync(Guid chatId, string username);
        Task<HashSet<UserDTO>> GetChatUsersAsync(Guid chatId);
        Task<bool> DeleteUserFromChat(Guid chatId, Guid userId, string removedUser, string adminName);


    }
}
