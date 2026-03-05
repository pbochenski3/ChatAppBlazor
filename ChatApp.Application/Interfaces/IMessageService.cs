using ChatApp.Application.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Interfaces
{
    public interface IMessageService
    {
        Task SendMessageAsync(MessageDTO dto);

        Task<List<MessageDTO>> GetMessagesHistoryAsync(int count);
    }
}
