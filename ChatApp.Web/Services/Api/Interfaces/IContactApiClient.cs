using ChatApp.Application.DTO;
using ChatApp.Application.DTO.Chats;
using ChatApp.Application.DTO.Result;

namespace ChatApp.Web.Services.Api.Interfaces
{
    public interface IContactApiClient
    {
        Task<List<ContactDTO>> GetContactListAsync(Guid chatId);
        Task RemoveContactAsync(Guid chatId);
        Task<List<UserChatDTO>> GetSidebarItemsAsync();
        Task<List<UserSearchResultDTO>> GetSearchedUsersList(string query);
    }
}
