using ChatApp.Application.DTO;
using ChatApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Interfaces.Service
{
    public interface IChatService
    {
        Task<bool> GetChatStatus(Guid ChatId, Guid ContactId);
    }
}
