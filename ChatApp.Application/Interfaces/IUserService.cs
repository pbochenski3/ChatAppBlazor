using ChatApp.Application.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Interfaces
{
    public interface IUserService
    {
        Task Register(UserDTO user);
        Task<UserDTO> Login(UserDTO user);
    }
}
