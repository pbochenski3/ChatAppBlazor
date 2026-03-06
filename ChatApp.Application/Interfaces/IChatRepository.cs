using ChatApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Interfaces
{
    public interface IChatRepository
    {
        Task AddChatAsync(Chat chat);
        Task SaveChangesToDbAsync();
    }
}
