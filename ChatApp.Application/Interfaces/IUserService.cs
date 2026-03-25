using ChatApp.Application.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Interfaces.Service
{
    public interface IUserService
    {
        Task Register(UserDTO user);
        Task<UserDTO> Login(UserDTO user);
        Task<List<UserDTO>> GetAllUsersToInvite(Guid currentUserId, string query);
        Task<HashSet<UserDTO>> GetUsersByIdAsync(HashSet<Guid> Ids);
        Task<UserDTO> GetUserDtoAsync(Guid userId);
    }
}
