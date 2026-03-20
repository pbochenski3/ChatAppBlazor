using ChatApp.Application.DTO;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace ChatApp.Application.Interfaces.Service
{
    public interface IMessageService
    {
        Task SendChatMessageAsync(MessageDTO dto);
        Task<List<MessageDTO>> GetPrivateHistoryAsync(Guid userId, Guid chatId,CancellationToken token);
    }
}
