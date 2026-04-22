using ChatApp.Application.DTO;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatApp.Application.Interfaces.Service
{
    public interface IUserService
    {
        Task RegisterUserAsync(UserDTO userDto);
        Task<UserDTO?> LoginUserAsync(UserDTO userDto);
        Task<List<UserDTO>> GetUsersToInviteAsync(Guid currentUserId, string query);
        //Task<List<UserDTO>> GetUsersByIdsAsync(HashSet<Guid> userIds);
        //Task<UserDTO?> GetUserByIdAsync(Guid userId);
        Task UpdateUserAvatarAsync(Guid userId, IFormFile avatarFile);
        Task<string> GetAvatarUrlAsync(Guid userId);

    }
}
