namespace ChatApp.Application.Interfaces.Chats
{
    public interface IChatService
    {
        Task UpdateGroupAvatarUrl(Guid chatId, string avatarUrl);
        //Task<HashSet<Guid>> GetUsersInChatIdAsync(Guid chatId);
        Task DeleteChatAsync(Guid chatId, Guid userId);
        Task<bool> IsChatExistingAsync(Guid chatId, Guid userId);
        Task<string> GetGroupAvatarUrlAsync(Guid chatId);
    }
}
