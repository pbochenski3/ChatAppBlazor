using ChatApp.Domain.Models;
using ChatApp.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Infrastructure.Persistence
{
    public class UserChatRepository : IUserChatRepository
    {
        private readonly IDbContextFactory<ChatDbContext> _contextFactory;
        private readonly ILogger<UserChatRepository> _logger;

        public UserChatRepository(IDbContextFactory<ChatDbContext> contextFactory, ILogger<UserChatRepository> logger)
        {
            _contextFactory = contextFactory;
            _logger = logger;
        }
        public async Task SaveLastSendedChatMessage(Guid chatId, Guid messageId)
        {
            using var context = _contextFactory.CreateDbContext();
            var affected = await context.UserChat
                .Where(uc => uc.ChatID == chatId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(uc => uc.LastMessageID, messageId)
                    .SetProperty(uc => uc.LastMessageAt, DateTime.UtcNow)
                );
            _logger.LogInformation("SaveLastSendedChatMessage: updated {Count} UserChat rows for ChatID={ChatId} with MessageID={MessageId}", affected, chatId, messageId);
        }
        public async Task SaveLastReadMessage(Guid userId,Guid chatId,Guid messageId)
        {
            using var context = _contextFactory.CreateDbContext();
            await context.UserChat
                .Where(uc => uc.UserID == userId && uc.ChatID == chatId)
                .ExecuteUpdateAsync(s => s
                .SetProperty(uc => uc.LastReadMessageID, messageId)
                .SetProperty(uc => uc.LastReadAt, DateTime.UtcNow)
                );
        }
        public async Task<int> CountUnreadMessagesAsync(Guid userId,Guid chatId)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.UserChat
                 .Where(uc => uc.UserID == userId && uc.ChatID == chatId)
                 .Select(uc => uc.Chat.Messages
                     .Count(m => m.SentAt > uc.LastReadAt
                              && m.SenderID != userId
                              && !m.IsDeleted)) 
                 .FirstOrDefaultAsync();
        }
        public async Task<List<CounterBadge>> CountAllUnreadMessagesAsync(Guid userId)
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.UserChat
                .Where(uc => uc.UserID == userId)
                .Select(uc => new CounterBadge(
                    uc.ChatID,
                    uc.Chat.Messages.Count(m =>
                        m.SentAt > uc.LastReadAt &&
                        m.SenderID != userId &&
                        !m.IsDeleted)
                ))
                .ToListAsync();
        }
    }
}
