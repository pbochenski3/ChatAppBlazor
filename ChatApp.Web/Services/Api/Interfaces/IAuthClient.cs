using ChatApp.Application.DTO;

namespace ChatApp.Web.Services.Api.Interfaces
{
    public interface IAuthClient
    {
        Task LoginUserAsync(UserDTO dto);
        Task RegisterUserAsync(UserDTO dto);
    }
}
