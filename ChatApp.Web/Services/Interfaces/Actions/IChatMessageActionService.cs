namespace ChatApp.Web.Services.Interfaces.Actions
{
    public interface IChatMessageActionService
    {
        public event Action? OnStateChanged;
        Task HandleMessageEditedAsync(Guid messageId, Guid chatId, string content);
        Task HandleMessageDeletedAsync(Guid messageId, Guid chatId);
        Task RequestMessageDeleted(Guid messageId, Guid chatId);
        Task RequestMessageEdited(Guid messageId, Guid chatId, string content);

    }
}
