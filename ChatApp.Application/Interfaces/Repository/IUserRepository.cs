using ChatApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Interfaces.Repository
{
    public interface IUserRepository
    {
        Task RegisterAsync(User user);
        Task SaveChangesToDbAsync();
        //Task LoginAsync(User user);
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByIdAsync(Guid id);
        Task<List<User>> GetAllUsersToInviteAsync(Guid currentUserId, string query);
        Task SetStatus(Guid id, bool status);
    }
}
