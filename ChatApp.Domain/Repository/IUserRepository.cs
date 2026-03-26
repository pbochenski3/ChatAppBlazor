using ChatApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatApp.Application.Interfaces.Repository
{
    public interface IUserRepository
    {
        Task RegisterAsync(User user);
        Task SaveChangesToDbAsync();
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByIdAsync(Guid id);
        Task<List<User>> GetAllUsersToInviteAsync(Guid currentUserId, string query);
        Task SetUserStatusAsync(Guid id, bool status);
        Task<HashSet<User>> GetUsersByIdsAsync(HashSet<Guid> ids);
    }
}
