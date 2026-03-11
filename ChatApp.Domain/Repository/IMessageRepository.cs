using ChatApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Interfaces.Repository
{
    public interface IMessageRepository
    {
        Task AddAsync(Message message);
        Task SaveChangesAsync();
        Task<List<Message>> GetMessageHistoryAsync(Guid contactId, Guid id,Guid chatId);
    }
}
