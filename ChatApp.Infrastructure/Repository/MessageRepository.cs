using ChatApp.Domain.Entities;
using ChatApp.Domain.Interfaces.Repository;
using ChatApp.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ChatApp.Infrastructure.Repository
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
            if (ids == null || !ids.Any())
                return new Dictionary<Guid, MessagePreview>();

            return await _context.Messages
                .IgnoreQueryFilters()
                .AsNoTracking()
                .Where(m => ids.Contains(m.MessageID))
                .Select(m => new
                {
                    m.MessageID,
                    Preview = new MessagePreview
                    {
                        Content = m.Content,
                        Author = m.Sender.UserChats
                            .Where(u => u.Alias != null)
                            .Select(u => u.Alias)
                            .FirstOrDefault() ?? string.Empty,
                        SenderId = m.SenderID,
                    }
                })
                .ToDictionaryAsync(x => x.MessageID, x => x.Preview);
        }
        public async Task<List<Message>> GetMessageHistoryAsync(Guid chatId, DateTime? cutoffDate, CancellationToken token)
        {
            var query = _context.Messages
                .IgnoreQueryFilters()
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
        public async Task<bool> DeleteMessageAsync(Guid messageId, Guid chatId, Guid userId)
        {
            var message = await _context.Messages
            .Include(m => m.History)
            .Where(m => m.ChatID == chatId)
            .FirstOrDefaultAsync(m => m.MessageID == messageId);
            int version = message.History.Any()
            ? message.History.Max(m => m.Version) + 1
            : 1;
            var permission = message.SenderID == userId;
            if (permission == false) return false;

            await _context.MessagesHistory.AddAsync(
               new MessageHistory
               {
                   MessageId = message.MessageID,
                   Version = version,
                   EditedAt = DateTime.UtcNow,
                   OldContent = message.Content,
               });
            var rowsAffected = await _context.Messages.Where(m => m.MessageID == messageId && m.ChatID == chatId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(m => m.IsDeleted, true)
                    .SetProperty(uc => uc.DeletedAt, DateTime.UtcNow)
                    .SetProperty(uc => uc.Content, "Wiadomość została usunięta")
                );
            return rowsAffected > 0 ? true : false;
        }
        public async Task<bool> UpdateMessageContentAsync(Guid messageId, Guid chatId, string content, Guid userId, DateTime editTime)
        {
            var message = await _context.Messages
            .Include(m => m.History)
            .Where(m => m.ChatID == chatId)
            .FirstOrDefaultAsync(m => m.MessageID == messageId);
            if (message == null) return false;
            var permission = message.SenderID == userId;
            if (permission == false) return false;
            int nextVersion = message.History.Any()
                ? message.History.Max(e => e.Version) + 1
                : 1;
            await _context.MessagesHistory.AddAsync(
                new MessageHistory
                {
                    MessageId = messageId,
                    Version = nextVersion,
                    EditedAt = editTime,
                    OldContent = message.Content,
                });

            await _context.Messages.Where(m => m.MessageID == messageId)
                .ExecuteUpdateAsync(s =>
                s.SetProperty(m => m.Content, content)
                .SetProperty(m => m.IsEdited, true));

            return true;
        }
    }

}
