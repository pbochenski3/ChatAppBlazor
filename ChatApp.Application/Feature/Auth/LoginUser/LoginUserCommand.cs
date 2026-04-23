using ChatApp.Application.Common.Messaging;
using ChatApp.Application.DTO;

namespace ChatApp.Application.Feature.Auth.LoginUser
{
    public record LoginUserCommand(UserDTO LoginData) : ICommand<UserDTO>;
}
