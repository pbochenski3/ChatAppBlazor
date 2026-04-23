using ChatApp.Application.Common.Messaging;
using ChatApp.Application.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace ChatApp.Application.Feature.Auth.LoginUser
{
    public record LoginUserCommand(UserDTO LoginData) : ICommand<UserDTO>;
}
