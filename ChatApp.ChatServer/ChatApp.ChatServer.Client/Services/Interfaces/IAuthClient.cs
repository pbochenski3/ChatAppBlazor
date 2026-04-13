using ChatApp.Application.DTO;

namespace ChatApp.ChatServer.Client.Services.Interfaces
{
    public interface IAuthClient
    {
        Task LoginUserAsync(UserDTO dto);
        Task RegisterUserAsync(UserDTO dto);
    }
}
