using ChatApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Interfaces
{
    public interface IUserRepository
    {
        Task RegisterAsync(User user);
        Task SaveChangesToDbAsync();
        //Task LoginAsync(User user);
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByIdAsync(Guid id);
    }
}
