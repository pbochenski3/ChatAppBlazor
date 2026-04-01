using ChatApp.Domain.Models;
using ChatApp.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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

        public async Task SetChatAccessibilityAsync(Guid chatId, bool active, HashSet<Guid>? userIds = null)
        {
            using var context = _contextFactory.CreateDbContext();
            var query = context.UserChat.IgnoreQueryFilters().Where(uc => uc.ChatID == chatId);
            if (userIds != null && userIds.Any())
            {
                query = query.Where(uc => userIds.Contains(uc.UserID));
            }

            if (active)
            {
                await query.ExecuteUpdateAsync(s => s
                    .SetProperty(uc => uc.IsArchive, false)
                    .SetProperty(uc => uc.IsDeleted, false)
                    .SetProperty(uc => uc.ArchivedAt, (DateTime?)null)
                    .SetProperty(uc => uc.DeletedAt, (DateTime?)null));
            }
            else
            {
                await query.ExecuteUpdateAsync(s => s
                    .SetProperty(uc => uc.IsArchive, true)
                    .SetProperty(uc => uc.ArchivedAt, DateTime.UtcNow));
            }
        }


        public async Task<bool> ExistsAsync(Guid chatId)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.UserChat.AnyAsync(c => c.ChatID == chatId);
        }

        public async Task<bool> IsChatArchivedAsync(Guid chatId, Guid userId)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.UserChat
                .AnyAsync(c => c.ChatID == chatId &&
                               c.UserID == userId &&
                               c.IsArchive == true);
        }

        public async Task<Guid> GetReceiverUserIdAsync(Guid chatId, Guid userId, CancellationToken token)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.UserChat
                .AsNoTracking()
                .Where(uc => uc.ChatID == chatId && uc.UserID != userId)
                .Select(uc => uc.UserID)
                .FirstOrDefaultAsync(token);
        }

        public async Task UpdateLastSentMessageAsync(Guid chatId, Guid messageId)
        {
            using var context = _contextFactory.CreateDbContext();
            var affected = await context.UserChat
                .Where(uc => uc.ChatID == chatId && uc.IsArchive == false)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(uc => uc.LastMessageID, messageId)
                    .SetProperty(uc => uc.LastMessageAt, DateTime.UtcNow)
                );
            _logger.LogInformation("UpdateLastSentMessageAsync: updated {Count} UserChat rows for ChatID={ChatId}", affected, chatId);
        }

        public async Task UpdateLastReadMessageAsync(Guid userId, Guid chatId, Guid messageId)
        {
            using var context = _contextFactory.CreateDbContext();
            await context.UserChat
                .Where(uc => uc.UserID == userId && uc.ChatID == chatId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(uc => uc.LastReadMessageID, messageId)
                    .SetProperty(uc => uc.LastReadAt, DateTime.UtcNow)
                );
        }

        public async Task<int> CountUnreadMessagesAsync(Guid userId, Guid chatId)
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

        public async Task<UserChat?> GetUserChatAsync(Guid chatId, Guid userId, CancellationToken token)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.UserChat
               .Include(ch => ch.Chat)
                     .ThenInclude(c => c.UserChats)
                          .ThenInclude(uc => uc.User)
               .IgnoreQueryFilters()
               .Where(c => c.ChatID == chatId && c.UserID == userId)
               .FirstOrDefaultAsync(token);
        }

        public async Task<List<UserChat>?> GetAllUserChatsAsync(Guid userId)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.UserChat
                .Include(uc => uc.Chat)
                    .ThenInclude(c => c.UserChats)
                        .ThenInclude(uc => uc.User)
                .Where(uc => uc.UserID == userId)
                .ToListAsync();
        }

        public async Task<HashSet<Guid>> GetUsersInChatAsync(Guid chatId)
        {
            using var context = _contextFactory.CreateDbContext();
            var userIds = await context.UserChat
                .AsNoTracking()
                .Where(uc => uc.ChatID == chatId && uc.IsArchive == false)
                .Select(uc => uc.UserID)
                .ToListAsync();
            return userIds.ToHashSet();
        }

        public async Task ArchiveChatAsync(Guid chatId, Guid userId)
        {
            using var context = _contextFactory.CreateDbContext();
            await context.UserChat
                .Where(ch => ch.ChatID == chatId && ch.UserID == userId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(ch => ch.IsArchive, true)
                    .SetProperty(ch => ch.ArchivedAt, DateTime.UtcNow));
        }

        public async Task<DateTime?> GetLastReadAtAsync(Guid userId, Guid chatId)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.UserChat
                .Where(uc => uc.ChatID == chatId && uc.UserID == userId)
                .Select(uc => (DateTime?)uc.LastReadAt)
                .FirstOrDefaultAsync();
        }

        public async Task MarkChatAsDeletedAsync(Guid chatId, Guid userId)
        {
            using var context = _contextFactory.CreateDbContext();
            await context.UserChat
                .Where(uc => uc.ChatID == chatId && uc.UserID == userId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(uc => uc.IsDeleted, true)
                    .SetProperty(uc => uc.ArchivedAt, DateTime.UtcNow));
        }

        public async Task<List<(Guid ChatId, int Count)>> CountAllUnreadMessageCountsAsync(Guid userId)
        {
            using var context = _contextFactory.CreateDbContext();
            var rawData = await context.UserChat
                .Where(uc => uc.UserID == userId)
                .Select(uc => new
                {
                    Id = uc.ChatID,
                    UnreadCount = uc.Chat.Messages.Count(m =>
                        m.SentAt > uc.LastReadAt &&
                        m.SenderID != userId &&
                        !m.IsDeleted)
                })
                .ToListAsync();
            return rawData.Select(x => (x.Id, x.UnreadCount)).ToList();
        }
        public async Task UnarchiveChatAsync(Guid chatId, HashSet<Guid> userIds)
        {
            using var context = _contextFactory.CreateDbContext();
            await context.UserChat
                .Where(uc => uc.ChatID == chatId && userIds.Contains(uc.UserID))
                .ExecuteUpdateAsync(s => s
                    .SetProperty(uc => uc.IsArchive, false)
                    .SetProperty(uc => uc.ArchivedAt, (DateTime?)null));
        }

        public async Task ArchivePrivateChatAsync(Guid chatId, Guid userId, Guid contactId)
        {
            using var context = _contextFactory.CreateDbContext();
            var affected = await context.UserChat
                .IgnoreQueryFilters()
                .Where(uc => uc.ChatID == chatId && (uc.UserID == userId || uc.UserID == contactId))
                .ExecuteUpdateAsync(s => s
                    .SetProperty(uc => uc.IsArchive, true)
                    .SetProperty(uc => uc.ArchivedAt, DateTime.UtcNow));

            _logger.LogInformation("ArchivePrivateChat: set IsArchive=true for chat {ChatId}. Rows affected: {Count}", chatId, affected);
        }

        public async Task<bool> GetChatStatusById(Guid chatId, Guid userId)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.UserChat
                .Where(uc => uc.ChatID == chatId && uc.UserID == userId)
                .Select(uc => uc.IsArchive)
                .FirstOrDefaultAsync();
        }
    }
}
