using ChatApp.Application.Interfaces;
using ChatApp.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ChatApp.Infrastructure.Persistence;

public class MessageRepository : IMessageRepository
{
    private readonly ChatDbContext _context;
    private readonly ILogger<MessageRepository> _logger;

    public MessageRepository(ChatDbContext context,ILogger<MessageRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task AddAsync(Message message)
    {
        _logger.LogInformation("Adding a new message to the database:");
        await _context.Messages.AddAsync(message);
    }

    public async Task SaveChangesAsync()
    {
        _logger.LogInformation("Saving changes to the database.");
        await _context.SaveChangesAsync();
    }
    public async Task<List<Message>> GetRecentMessagesAsync(int count)
    {
        var messages = await _context.Messages
            .Include(m => m.Sender) 
            .OrderByDescending(m => m.SentAt) 
            .ToListAsync(); 

        return messages.OrderBy(m => m.SentAt).ToList();
    }
}
