using ChatApp.Application.DTO;
using ChatApp.Application.Interfaces.Repository;
using ChatApp.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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
    public async Task RestoreChat(Guid chatId)
    {
        using var context = _contextFactory.CreateDbContext();
        await context.UserChat
            .IgnoreQueryFilters()
            .Where(uc => uc.ChatID == chatId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(uc => uc.IsDeleted, false)
                .SetProperty(uc => uc.DeletedAt, (DateTime?)null)
                .SetProperty(uc => uc.IsArchive, false));

    }
    public async Task<Chat?> GetPrivateChat(Guid user1, Guid user2)
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.Chats
            .IgnoreQueryFilters()
            .Include(c => c.UserChats)
            .Where(c => !c.IsGroup) 
            .Where(c => c.UserChats.Any(uc => uc.UserID == user1))
            .Where(c => c.UserChats.Any(uc => uc.UserID == user2))
            .FirstOrDefaultAsync();
    }
    public async Task AddChatAsync(Chat chat)
    {
        using var context = _contextFactory.CreateDbContext();
        await context.Chats.AddAsync(chat);
        _logger.LogInformation("Adding a new chat to the database:");
        await context.SaveChangesAsync();
    }
    public async Task ArchivePrivateChatFromDb(Guid chatId,Guid userId, Guid contactId)
    {
        using var context = _contextFactory.CreateDbContext();
        var userExistsInChat = await context.UserChat
       .AnyAsync(uc => uc.ChatID == chatId && uc.UserID == userId);

        if (userExistsInChat)
        {
            await context.UserChat
             .Where(uc => uc.ChatID == chatId && uc.UserID == userId)
             .ExecuteUpdateAsync(s => s
                 .SetProperty(uc => uc.IsDeleted, true)
                 .SetProperty(uc => uc.DeletedAt, DateTime.UtcNow));

            await context.UserChat
            .Where(uc => uc.ChatID == chatId && uc.UserID == contactId)
            .ExecuteUpdateAsync(s => s
                .SetProperty(uc => uc.IsArchive, true));
            await context.SaveChangesAsync();
        }
    }
    public async Task DeletePrivateChat(Guid chatId)
    {

    }
    public async Task SaveChangesToDbAsync()
    {
        using var context = _contextFactory.CreateDbContext();
        _logger.LogInformation("Saving changes to the database.");
        await context.SaveChangesAsync();
    }
}
