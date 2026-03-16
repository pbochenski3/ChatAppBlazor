using ChatApp.Application.DTO;
using ChatApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Interfaces.Service
{
    public interface IChatService
    {
        Task<ChatDTO> GetPrivateChatById(Guid contactId, Guid currenUserId);
        Task<Chat> CreateChat(List<User> users, string ChatName, bool isGroup);
        Task<bool> GetChatStatus(Guid ChatId, Guid ContactId);
        Task<List<ChatDTO>> GetChatList(Guid UserId);
    }
}
