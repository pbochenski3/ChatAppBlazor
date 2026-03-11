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
    public async Task<Chat?> GetChatById(Guid user1, Guid user2)
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.ChatUsers
          .Include(uc => uc.Chat)
          .Where(uc => uc.UserID == user1 || uc.UserID == user2)
          .GroupBy(uc => uc.ChatID)
          .Where(g => g.Count() == 2)
          .Select(g => g.First().Chat) 
          .FirstOrDefaultAsync();
    }
    public async Task AddChatAsync(Chat chat)
    {
        using var context = _contextFactory.CreateDbContext();
        await context.Chats.AddAsync(chat);
        _logger.LogInformation("Adding a new chat to the database:");
        await context.SaveChangesAsync();
    }
    public async Task SaveChangesToDbAsync()
    {
        using var context = _contextFactory.CreateDbContext();
        _logger.LogInformation("Saving changes to the database.");
        await context.SaveChangesAsync();
    }
}
