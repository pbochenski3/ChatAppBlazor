using ChatApp.Application.Interfaces;
using ChatApp.Domain.Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

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
    public async Task<User?> GetByIdAsync(int id)
    {
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserID == id);
        }
    }

    public async Task SaveChangesAsync()
    {
        _logger.LogInformation("Saving changes to the database.");
        await _context.SaveChangesAsync();
    }

}
