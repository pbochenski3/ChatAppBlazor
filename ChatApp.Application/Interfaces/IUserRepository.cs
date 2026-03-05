using ChatApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Interfaces
{
    public interface IUserRepository
    {
        Task RegisterAsync(User user);
        Task SaveChangesAsync();
    }
}
