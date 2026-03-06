using ChatApp.Application.Interfaces;
using ChatApp.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ChatApp.Infrastructure.Persistence;

public class ChatRepository : IChatRepository
{
    private readonly ChatDbContext _context;
    private readonly ILogger<MessageRepository> _logger;

    public ChatRepository(ChatDbContext context, ILogger<MessageRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public Task AddChatAsync(Chat chat)
    {
        _logger.LogInformation("Adding a new chat to the database:");
        return _context.Chats.AddAsync(chat).AsTask();
    }
    public async Task SaveChangesToDbAsync()
    {
        _logger.LogInformation("Saving changes to the database.");
        await _context.SaveChangesAsync();
    }
}
