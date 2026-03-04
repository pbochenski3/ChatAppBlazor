using ChatApp.Application.Interfaces;
using ChatApp.Domain.Models;
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
        _logger.LogInformation("Adding a new message to the database: " +
    "SenderID: {SenderID}, ChatID: {ChatID}, Content: {Content}",
    message.SenderID, message.ChatID, message.Content);
        //await _context.Messages.AddAsync(message);
    }

    public async Task SaveChangesAsync()
    {
        _logger.LogInformation("Saving changes to the database.");
        //await _context.SaveChangesAsync();
    }
}
