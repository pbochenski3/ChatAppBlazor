namespace ChatApp.Application.Interfaces.Chats
{
    public interface IChatReadStatusService
    {

        Task<int> GetUnreadMessageCountAsync(Guid userId, Guid chatId);
        Task<List<(Guid ChatId, int Count)>> GetAllUnreadMessageCountsAsync(Guid userId);
        Task<DateTime?> GetLastMessageAtChatAsync(Guid userId, Guid chatId);
        Task SaveLastSentMessageIdAsync(Guid chatId, Guid messageId);
    }
}
