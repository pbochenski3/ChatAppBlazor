using ChatApp.Application.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Interfaces.Service
{
    public interface IChatService
    {
        Task<ChatDTO> GetChatById(Guid contactId, Guid currenUserId);
        Task CreateChat(Guid contactId, Guid id);
    }
}
