using ChatApp.Application.Interfaces.Repository;
using ChatApp.Domain.Models;
using ChatApp.Infrastructure.Persistence;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChatApp.Infrastructure.Persistence
{
    public class ChatRepository : IChatRepository
    {
        private readonly IDbContextFactory<ChatDbContext> _contextFactory;
        private readonly ILogger<ChatRepository> _logger;

        public ChatRepository(IDbContextFactory<ChatDbContext> contextFactory, ILogger<ChatRepository> logger)
        {
            _contextFactory = contextFactory;
            _logger = logger;
        }

        public async Task<bool> CheckIfGroupExist(Guid chatId, Guid userId)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Chats
                .AnyAsync(ch => ch.ChatID == chatId
                             && ch.UserChats.Any(uc => uc.UserID == userId)
                             && ch.IsGroup == true);
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

        public async Task AddUserGroupToDb(Guid chatId, HashSet<Guid> userIdsToAdd)
        {
            using var context = _contextFactory.CreateDbContext();

            var chat = await context.Chats
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(c => c.ChatID == chatId);

            if (chat == null) return;

            if (chat.IsDeleted)
            {
                chat.IsDeleted = false;
                chat.DeletedAt = null;
                await context.SaveChangesAsync();
            }

            var existingUserIds = await context.UserChat
                .IgnoreQueryFilters()
                .Where(uc => uc.ChatID == chatId)
                .Select(uc => uc.UserID)
                .ToListAsync();

            var filteredUsers = userIdsToAdd
                .Where(id => !existingUserIds.Contains(id))
                .Select(userId => new UserChat
                {
                    UserID = userId,
                    ChatID = chat.ChatID,
                    ChatName = chat.ChatName,
                    IsArchive = false
                }).ToList();

            if (filteredUsers.Any())
            {
                await context.UserChat.AddRangeAsync(filteredUsers);
                await context.SaveChangesAsync();
            }
        }

        public async Task<Chat?> FetchChatById(Guid chatId)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Chats
                .Include(uc => uc.UserChats)
                .FirstOrDefaultAsync(c => c.ChatID == chatId);
        }

        public async Task<Guid> GetChatIdAsync(Guid userId1, Guid userId2, CancellationToken token = default)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Chats
                .Where(c => !c.IsGroup &&
                            c.UserChats.Any(uc => uc.UserID == userId1) &&
                            c.UserChats.Any(uc => uc.UserID == userId2))
                .Select(c => c.ChatID)
                .FirstOrDefaultAsync(token);
        }

        public async Task AddChatAsync(Chat chat)
        {
            using var context = _contextFactory.CreateDbContext();
            await context.Chats.AddAsync(chat);
            _logger.LogInformation("Adding a new chat to the database: {ChatId}", chat.ChatID);
            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                var message = ex.InnerException?.Message ?? ex.Message;
                _logger.LogError("Błąd zapisu czatu: {Message}", message);
                throw new HubException("Nie udało się zapisać czatu.");
            }
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

        public async Task<HashSet<Guid>> GetExistingUsersInChat(Guid chatId, HashSet<Guid> userIdsToCheck)
        {
            using var context = _contextFactory.CreateDbContext();
            var existingIds = await context.UserChat
                .IgnoreQueryFilters()
                .Where(uc => uc.ChatID == chatId && userIdsToCheck.Contains(uc.UserID))
                .Select(uc => uc.UserID)
                .ToListAsync();

            return existingIds.ToHashSet();
        }

        public async Task TryDeleteChatIfEmptyAsync(Guid chatId)
        {
            using var context = _contextFactory.CreateDbContext();

            var hasMembers = await context.UserChat.AnyAsync(uc => uc.ChatID == chatId && !uc.IsDeleted);
            if (!hasMembers)
            {
                await context.Chats
                    .Where(ch => ch.ChatID == chatId)
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(ch => ch.IsDeleted, true)
                        .SetProperty(ch => ch.DeletedAt, DateTime.UtcNow));
            }
        }
    }
}
