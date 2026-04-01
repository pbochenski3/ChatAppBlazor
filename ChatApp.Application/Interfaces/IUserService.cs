using ChatApp.Application.DTO;
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
        Task<HashSet<UserDTO>> GetUsersByIdsAsync(HashSet<Guid> userIds);
        Task<UserDTO?> GetUserByIdAsync(Guid userId);
        Task UpdateUserAvatarAsync(Guid userId, string avatarUrl);
        Task<string> GetAvatarUrlAsync(Guid userId);

    }
}
