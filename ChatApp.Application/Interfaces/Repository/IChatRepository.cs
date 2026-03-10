using ChatApp.Application.DTO;
using ChatApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Interfaces.Repository
{
    public interface IChatRepository
    {
        Task AddChatAsync(Chat chat);
        Task SaveChangesToDbAsync();
        Task<ChatDTO?> GetChatById(Guid user1, Guid user2);
    }
}
