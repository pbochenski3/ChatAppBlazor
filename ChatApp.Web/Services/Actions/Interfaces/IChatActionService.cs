using ChatApp.Application.DTO;
using ChatApp.Application.Events;

namespace ChatApp.Web.Services.Actions.Interfaces
{
    public interface IChatActionService
    {
        event Action? OnStateChanged;
        event Func<Guid, Task>? OnJoinGroupRequested;
        Task HandleIncomingMessageAsync(MessageDTO dto);
        void MarkAsRead(Guid chatId, Guid messageId);
        Task HandleChatLoadAsync(ContactSelectedArgs args);
        Task Refresh();
        Task HandleUserOnGroupLoadAsync(Guid chatId);
        Task HandleChatCloseAsync();



    }
}
