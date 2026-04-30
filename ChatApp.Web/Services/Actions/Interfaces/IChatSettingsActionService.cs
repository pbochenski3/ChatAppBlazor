using ChatApp.Domain.Enums;

namespace ChatApp.Web.Services.Actions.Interfaces
{
    public interface IChatSettingsActionService
    {
        event Action? OnStateChanged;
        void RequestUserRemove();
        Task HandleRemoveUserFromChat();
        Task HandleChatDeleteAsync();
        void RequestLeaveChat();
        Task HandleChatLeaveAsync();
        void RequestContactDelete(Guid chatId);
        Task HandleContactDeleteAsync(Guid chatId);
        Task HandleLoadUsersToAddAsync();
        Task HandleChangeChatNameAsync(string chatName);
        Task HandleAddUsersToChatAsync(HashSet<Guid> usersToAdd, AddType type);
        Task OpenUserDetails(Guid userId, string alias, string username);
        void CloseUserDetails();
    }
}
