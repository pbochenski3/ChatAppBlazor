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
        private readonly ChatDbContext _context;
        private readonly ILogger<ChatRepository> _logger;

        public ChatRepository(ChatDbContext context, ILogger<ChatRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<bool> CheckIfChatIsArchive(Guid chatId,Guid userId)
        {
            bool? result =  await _context.UserChat
                .Where(ch => ch.ChatID == chatId && ch.UserID == userId)
                .Select(ch => ch.IsArchive)
                .FirstOrDefaultAsync();
            return result ?? false;
        }
        public async Task<bool> CheckIfGroupExist(Guid chatId, Guid userId)
        {
            return await _context.Chats
                .AnyAsync(ch => ch.ChatID == chatId
                             && ch.UserChats.Any(uc => uc.UserID == userId)
                             && ch.IsGroup == true);
        }

        public async Task UpdateGroupAvatarUrl(Guid chatId, string avatarUrl)
        {
           await _context.Chats
                .Where(ch => ch.IsGroup && ch.ChatID == chatId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(ch => ch.AvatarUrl, avatarUrl));
        }
        public async Task AddUserGroupToDb(Guid chatId, HashSet<Guid> userIdsToAdd)
        {

            var chat = await _context.Chats
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(c => c.ChatID == chatId);

            if (chat == null) return;

            if (chat.IsDeleted)
            {
                chat.IsDeleted = false;
                chat.DeletedAt = null;
            }

            var existingUserIds = await _context.UserChat
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
                await _context.UserChat.AddRangeAsync(filteredUsers);
            }
        }

        public async Task<Chat?> FetchChatById(Guid chatId)
        {
            return await _context.Chats
                .FirstOrDefaultAsync(c => c.ChatID == chatId);
        }

        public async Task<Chat?> GetChatAsync(Guid userId1, Guid userId2, CancellationToken token = default)
        {
            return await _context.Chats
                .Where(c => !c.IsGroup &&
                            c.UserChats.Any(uc => uc.UserID == userId1) &&
                            c.UserChats.Any(uc => uc.UserID == userId2))
                .FirstOrDefaultAsync(token);
        }

        public async Task AddChatAsync(Chat chat)
        {
            _logger.LogInformation("Próba dodania czatu: {ChatName}", chat.ChatName);
            await _context.Chats.AddAsync(chat);
        }

        public async Task<string> GetGroupAvatarUrlAsync(Guid chatId)
        {
            return await _context.Chats
                .AsNoTracking()
                .Where(c => c.ChatID == chatId && c.IsGroup)
                .Select(c => c.AvatarUrl)
                .FirstOrDefaultAsync() ?? "https://localhost:7255/cdn/avatars/default-group-avatar.png";
        }
        public async Task<HashSet<Guid>> GetExistingUsersInChat(Guid chatId, HashSet<Guid> userIdsToCheck)
        {
            var existingIds = await _context.UserChat
                .IgnoreQueryFilters()
                .Where(uc => uc.ChatID == chatId && userIdsToCheck.Contains(uc.UserID))
                .Select(uc => uc.UserID)
                .ToListAsync();

            return existingIds.ToHashSet();
        }

        public async Task TryDeleteChatIfEmptyAsync(Guid chatId)
        {

            var hasMembers = await _context.UserChat.AnyAsync(uc => uc.ChatID == chatId && !uc.IsDeleted);
            if (!hasMembers)
            {
                await _context.Chats
                    .Where(ch => ch.ChatID == chatId)
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(ch => ch.IsDeleted, true)
                        .SetProperty(ch => ch.DeletedAt, DateTime.UtcNow));
            }
        }

        public async Task UpdateChatNameAsync(Guid chatId, string chatName)
        {
            await _context.Chats
                .Where(c => c.ChatID == chatId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(c => c.ChatName, chatName));
        }
        public async Task<bool> IsChatGroupAsync(Guid chatId)
        {
            return await _context.Chats
                .AnyAsync(c => c.ChatID == chatId &&
                               c.IsGroup == true);
        }
    }
}
