using ChatApp.Application.DTO;

namespace ChatApp.ChatServer.Client.Services.Api.Interfaces
{
    public interface IGroupChatApiClient
    {

        Task CreateGroupChatAsync(Guid chatId, HashSet<Guid> userIdsToAdd);
        Task AddUsersToGroupChatAsync(Guid chatId, HashSet<Guid> userIdsToAdd);
        Task LeaveGroupChatAsync(Guid chatId,string username);
        Task<HashSet<UserDTO>> GetChatUsersAsync(Guid chatId);

    }
}
