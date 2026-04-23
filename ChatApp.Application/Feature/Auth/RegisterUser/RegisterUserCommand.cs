using ChatApp.Application.Common.Messaging;
using ChatApp.Application.DTO;

namespace ChatApp.Application.Feature.Auth.RegisterUser
{
    public record RegisterUserCommand(UserDTO RegisterData) : ICommand<bool>;

}
