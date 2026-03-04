using ChatApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Interfaces
{
    public interface IMessageRepository
    {
        Task AddAsync(Message message);
        Task SaveChangesAsync();
    }
}
