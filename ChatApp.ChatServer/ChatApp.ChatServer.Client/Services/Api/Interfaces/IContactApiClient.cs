using ChatApp.Application.DTO;

namespace ChatApp.ChatServer.Client.Services.Api.Interfaces
{
    public interface IContactApiClient
    {
        Task<List<ContactDTO>> GetContactListAsync();
        Task RemoveContactAsync(Guid chatId);
    }
}
