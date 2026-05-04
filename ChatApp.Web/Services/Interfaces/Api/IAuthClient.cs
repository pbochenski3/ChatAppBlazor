using ChatApp.Application.DTO;

namespace ChatApp.Web.Services.Interfaces.Api
{
    public interface IAuthClient
    {
        Task LoginUserAsync(UserDTO dto);
        Task RegisterUserAsync(UserDTO dto);
    }
}
