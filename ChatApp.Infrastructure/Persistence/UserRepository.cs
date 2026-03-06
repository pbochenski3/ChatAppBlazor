using ChatApp.Application.DTO;
using ChatApp.Application.Interfaces.Repository;
using ChatApp.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ChatApp.Infrastructure.Persistence;

public class UserRepository : IUserRepository
{
    private readonly ChatDbContext _context;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(ChatDbContext context,ILogger<UserRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task RegisterAsync(User user)
    {
                await _context.Users.AddAsync(user);
    }
    public async Task<User?> GetByUsernameAsync(string username)
    {
        _logger.LogInformation("Retrieving user by username: {Username}", username);
        return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
    }
    public async Task SetStatus(Guid id,bool status)
    {
        await _context.Users.Where(u => u.UserID == id).ExecuteUpdateAsync(u => u.SetProperty(p => p.IsOnline, status));
    }
    public async Task<User?> GetByIdAsync(Guid id)
    {
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserID == id);
        }
    }
    public async Task<List<User>> GetAllUsersToInviteAsync(Guid currentUserId, string query)
    {
        _logger.LogInformation("Retrieving all users who can be invited with query: {Query}", query);

        return await _context.Users
            .Where(u => u.UserID != currentUserId && u.Username.Contains(query))
            .Where(u => !_context.Contacts.Any(c =>
                c.UserID == currentUserId && c.ContactUserID == u.UserID))
            .Where(u => !_context.Invites.Any(i =>
                ((i.SenderID == currentUserId && i.ReceiverID == u.UserID) ||
                 (i.SenderID == u.UserID && i.ReceiverID == currentUserId))
                 && i.Status == InviteStatus.Pending)) 
            .OrderBy(u => u.Username)
            .ToListAsync();
    }
    public async Task SaveChangesToDbAsync()
    {
        _logger.LogInformation("Saving changes to the database.");
        await _context.SaveChangesAsync();
    }

}
