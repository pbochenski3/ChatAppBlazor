using ChatApp.Application.Common.Messaging;
using ChatApp.Application.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Feature.Auth.RegisterUser
{
    public record RegisterUserCommand(UserDTO RegisterData) : ICommand<bool>;

}
