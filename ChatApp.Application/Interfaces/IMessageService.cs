using ChatApp.Application.DTO;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ChatApp.Application.Interfaces.Service
{
    public interface IMessageService
    {
        Task SaveMessageAsync(MessageDTO messageDto);
        Task<List<MessageDTO>> GetChatHistoryAsync(Guid userId, Guid chatId, CancellationToken token);
    
    }
}
