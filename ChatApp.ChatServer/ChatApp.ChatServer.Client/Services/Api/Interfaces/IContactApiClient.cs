using ChatApp.Application.DTO;
using ChatApp.Application.DTO.Chats;

namespace ChatApp.ChatServer.Client.Services.Api.Interfaces
{
    public interface IContactApiClient
    {
        Task<List<ContactDTO>> GetContactListAsync();
        Task RemoveContactAsync(Guid chatId);
        Task<List<UserChatDTO>> GetSidebarItemsAsync();
    }
}
