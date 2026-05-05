using ChatApp.Application.DTO;

namespace ChatApp.Web.Services.Interfaces.Actions
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
        Task HandleUserAliasChangeAsync(Guid chatId, Guid userId, string newAlias);
        Task HandleUpdateFlagOnChatAsync(Guid userId, Guid chatId, bool flag);



    }
}
