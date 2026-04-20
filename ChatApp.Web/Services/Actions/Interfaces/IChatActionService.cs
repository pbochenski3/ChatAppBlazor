using ChatApp.Application.DTO;

namespace ChatApp.Web.Services.Actions.Interfaces
{
    public interface IChatActionService
    {
        event Action? OnStateChanged;
        Task HandleIncomingMessageAsync(MessageDTO dto);
        void MarkAsRead(Guid chatId, Guid messageId);
        Task HandleChatLoadAsync(ContactSelectedArgs args);
        Task Refresh();
        Task HandleUserOnGroupLoadAsync(Guid chatId);
        Task HandleChatCloseAsync();



    }
}
