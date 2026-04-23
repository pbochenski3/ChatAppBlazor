using ChatApp.Application.DTO;
using ChatApp.Application.Interfaces.Repository;
using ChatApp.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ChatApp.Infrastructure.Persistence
{
    public class MessageRepository : IMessageRepository
    {
        private readonly ChatDbContext _context;
        private readonly ILogger<MessageRepository> _logger;

        public MessageRepository(ChatDbContext context, ILogger<MessageRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task AddMessageAsync(Message message)
        {
            _logger.LogInformation("Adding a new message to the database: {MessageID}", message.MessageID);
            await _context.Messages.AddAsync(message);
        }

        public async Task<Dictionary<Guid, MessagePreview>> GetMessagePreviewsAsync(List<Guid> ids)
        {
            return await _context.Messages
                .AsNoTracking()
                .Include(m => m.Sender)
                .Where(m => ids.Contains(m.MessageID))
                .ToDictionaryAsync(
                    m => m.MessageID,
                    m => new MessagePreview
                    {
                        Content = m.Content,
                        Author = m.Sender?.Username ?? ""
                    }
                );
        }
        public async Task<List<Message>> GetMessageHistoryAsync(Guid userId, Guid chatId, DateTime? cutoffDate, CancellationToken token)
        {
            var query = _context.Messages
                .AsNoTracking()
                .Include(m => m.Sender)
                .Where(m => m.ChatID == chatId);

            if (cutoffDate.HasValue)
            {
                query = query.Where(m => m.SentAt <= cutoffDate.Value);
            }

            return await query
                .OrderBy(m => m.SentAt)
                .ToListAsync(token);
        }
        public async Task UpdateImageUrlAsync(Guid messageId, string url)
        {
            await _context.Messages
                .Where(m => m.MessageID == messageId)
                .ExecuteUpdateAsync(s => s
                .SetProperty(m => m.imageUrl, url));

        }
    }

}
