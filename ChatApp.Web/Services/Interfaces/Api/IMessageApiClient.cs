namespace ChatApp.Web.Services.Interfaces.Api
{
    public interface IMessageApiClient
    {
        Task DeleteMessageAsync(Guid messageId, Guid chatId);
        Task EditMessageAsync(Guid messageId, Guid chatId, string content);
    }
}
