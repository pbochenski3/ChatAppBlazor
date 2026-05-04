using ChatApp.Application.Common.Messaging;
using ChatApp.Application.DTO;
using ChatApp.Application.DTO.Result;

namespace ChatApp.Application.Feature.Auth.RegisterUser
{
    public record RegisterUserCommand(UserDTO RegisterData) : ICommand<RegisterResult>;

}
