
using ChatApp.Application.Interfaces.Repository;
using ChatApp.Domain.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using System.Net.NetworkInformation;
using System.Reflection.Metadata.Ecma335;

namespace ChatApp.Infrastructure.Persistence;

public class ChatRepository : IChatRepository
{
    private readonly IDbContextFactory<ChatDbContext> _contextFactory;
    private readonly ILogger<MessageRepository> _logger;


    public ChatRepository(IDbContextFactory<ChatDbContext> contextFactory, ILogger<MessageRepository> logger)
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
    public async Task UnArchiveChat(Guid chatId, HashSet<Guid> usersId)
    {
        using var context = _contextFactory.CreateDbContext();
        await context.UserChat
            .Where(uc => uc.ChatID == chatId && usersId.Contains(uc.UserID))
            .ExecuteUpdateAsync(s => s
            .SetProperty(uc => uc.IsArchive, false)
            .SetProperty(uc => uc.ArchivedAt, (DateTime?)null));
    }
    public async Task AddUserGroupToDb(Guid chatId, HashSet<Guid> usersToAdd)
    {
        using var context = _contextFactory.CreateDbContext();


        var chat = await context.Chats
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(c => c.ChatID == chatId);

        if (chat == null) return;

        // Jeśli czat był oznaczony jako usunięty (bo był pusty), przywracamy go
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

        var filteredUsers = usersToAdd
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
    public async Task<Chat> FetchChatById(Guid chatId)
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.Chats
            .Include(uc => uc.UserChats)
            .FirstOrDefaultAsync(c => c.ChatID == chatId);
    }
    public async Task<Guid> GetChatIdAsync(Guid user1, Guid user2, CancellationToken token = default)
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.Chats
        .Where(c => !c.IsGroup &&
                    c.UserChats.Any(uc => uc.UserID == user1) &&
                    c.UserChats.Any(uc => uc.UserID == user2))
        .Select(c => c.ChatID)
        .FirstOrDefaultAsync(token);
    }
    public async Task AddChatAsync(Chat chat)
    {
        using var context = _contextFactory.CreateDbContext();
        await context.Chats.AddAsync(chat);
        _logger.LogInformation("Adding a new chat to the database:");
        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            var message = ex.InnerException?.Message ?? ex.Message;
            _logger.LogError($"Błąd zapisu czatu: {message}");
            throw new HubException("Nie udało się zapisac.");
        }
    }
    public async Task ArchivePrivateChat(Guid chatId, Guid userId, Guid contactId)
    {
        using var context = _contextFactory.CreateDbContext();
        var affected = await context.UserChat
            .IgnoreQueryFilters()
           .Where(uc => uc.ChatID == chatId && (uc.UserID == userId || uc.UserID == contactId))
            .ExecuteUpdateAsync(s => s
                .SetProperty(uc => uc.IsArchive, true)
                .SetProperty(uc => uc.ArchivedAt, DateTime.UtcNow));

        _logger.LogInformation("ArchivePrivateChat: set IsArchive=true for chat {ChatId}, user {UserId}. Rows affected: {Count}", chatId, userId, affected);
    }
    public async Task<bool> GetChatStatusById(Guid ChatId, Guid ContactId)
    {
        using var context = _contextFactory.CreateDbContext();
        var chat = await context.UserChat
            .Where(uc => uc.IsArchive)
            .Where(uc => uc.ChatID == ChatId)
            .Where(uc => uc.UserID == ContactId)
            .FirstOrDefaultAsync();
        return chat?.IsArchive ?? false;
    }
    public async Task<HashSet<Guid>> GetExistingUsersInChat(Guid chatId, HashSet<Guid> usersToCheck)
    {
        using var context = _contextFactory.CreateDbContext();
        var existingIds = await context.UserChat
            .IgnoreQueryFilters()
            .Where(uc => uc.ChatID == chatId && usersToCheck.Contains(uc.UserID))
            .Select(uc => uc.UserID)
            .ToListAsync();

        return existingIds.ToHashSet();
    }
    public async Task TryDeleteChatIfEmptyAsync(Guid chatId)
    {
        using var context = _contextFactory.CreateDbContext();
        await context.Chats
            .Where(ch => ch.ChatID == chatId && !ch.UserChats.Any())
            .ExecuteUpdateAsync(s => s
            .SetProperty(ch => ch.IsDeleted, true)
            .SetProperty(ch => ch.DeletedAt, DateTime.UtcNow));

    }

}
