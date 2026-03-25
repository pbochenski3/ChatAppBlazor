
using ChatApp.Application.Interfaces.Repository;
using ChatApp.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;

namespace ChatApp.Infrastructure.Persistence;

public class MessageRepository : IMessageRepository
{
    private readonly IDbContextFactory<ChatDbContext> _contextFactory;
    private readonly ILogger<MessageRepository> _logger;

    public MessageRepository(IDbContextFactory<ChatDbContext> contextFactory, ILogger<MessageRepository> logger)
    {
        _contextFactory = contextFactory;
        _logger = logger;
    }

    public async Task AddAsync(Message message)
    {
        using var context = _contextFactory.CreateDbContext();
        _logger.LogInformation("Adding a new message to the database: {MessageID}", message.MessageID);
        await context.Messages.AddAsync(message);
        await context.SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        using var context = _contextFactory.CreateDbContext();
        _logger.LogInformation("Saving changes to the database.");
        await context.SaveChangesAsync();
    }
    public async Task<List<Message>> GetMessageHistoryAsync(Guid userId, Guid chatId, DateTime? cutoffDate, CancellationToken token)
    {
        using var context = _contextFactory.CreateDbContext();
        var query = context.Messages
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
}
