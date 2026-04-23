using ChatApp.Application.DTO;
using ChatApp.Domain.Models;

namespace ChatApp.Application.Interfaces.Repository
{
    public interface IMessageRepository
    {
        Task AddMessageAsync(Message message);
        Task<List<Message>> GetMessageHistoryAsync(Guid userId, Guid chatId, DateTime? cutoffDate, CancellationToken token);
        Task<Dictionary<Guid, MessagePreview>> GetMessagePreviewsAsync(List<Guid> ids);
    }
}
