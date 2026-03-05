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
        {
           bool exists = await _context.Users.AnyAsync(u => u.Username == user.Username);
            if (exists) { 
                _logger.LogInformation("Username already exists.");
                throw new Exception("Username already exists.");
            }
                await _context.Users.AddAsync(user);
                _logger.LogInformation($"Registered Username: {user.Username} UserId: {user.UserID} Password: {user.Password}");
                _logger.LogInformation("User registered successfully.");
        }
    }

    public async Task SaveChangesAsync()
    {
        _logger.LogInformation("Saving changes to the database.");
        await _context.SaveChangesAsync();
    }
}
