using ChatApp.Application.DTO.Requests;

namespace ChatApp.Web.Services.Actions.Interfaces
{
    public interface ISidebarActionService
    {
        event Action? OnSidebarStateChanged;
        event Action? OnInvitesStateChanged;
        Task HandleChatsLoadAsync();
        Task HandleCounterUpdateAsync(Guid chatId, bool clean);
        Task HandleInviteResponseAsync(InviteActionRequest request);
        Task HandleGlobalSearchAsync(string query);
        Task HandleInvitesLoadAsync();
        Task HandleSendInviteAsync(Guid contactId);
        Task HandleSidebarLastMessageReloadAsync(Guid chatId, string sender, string content);
        Task HandleChatNameReloadAsync(Guid chatId, string newChatName);
        Task HandleSidebarLockAsync();
        Task Refresh();


    }
}
