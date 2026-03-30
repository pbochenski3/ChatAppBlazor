using ChatApp.Application.DTO;
using ChatApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ChatApp.Application.Interfaces.Repository
{
    public interface IMessageRepository
    {
        Task AddMessageAsync(Message message);
        Task SaveChangesAsync();
        Task<List<Message>> GetMessageHistoryAsync(Guid userId, Guid chatId, DateTime? cutoffDate, CancellationToken token);
        Task<Dictionary<Guid, MessagePreview>> GetMessagePreviewsAsync(List<Guid> ids);
    }
}
