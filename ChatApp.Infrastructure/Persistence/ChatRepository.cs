
using ChatApp.Application.Interfaces.Repository;
using ChatApp.Domain.Models;
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
    public async Task<Guid> GetChatIdAsync(Guid user1, Guid user2,CancellationToken token = default)
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
        await context.SaveChangesAsync();
    }
    public async Task ArchivePrivateChat(Guid chatId,Guid userId, Guid contactId)
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
    public async Task<bool> GetChatStatusById(Guid ChatId,Guid ContactId)
    {
        using var context = _contextFactory.CreateDbContext();
        var chat = await context.UserChat
            .Where(uc => uc.IsArchive)
            .Where(uc => uc.ChatID == ChatId)
            .Where(uc => uc.UserID == ContactId)
            .FirstOrDefaultAsync();
        return chat?.IsArchive ?? false;
    }
}
