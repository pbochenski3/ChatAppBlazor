
using ChatApp.Application.Interfaces.Repository;
using ChatApp.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using System.Net.NetworkInformation;

namespace ChatApp.Infrastructure.Persistence;

public class UserRepository : IUserRepository
{
    private readonly IDbContextFactory<ChatDbContext> _contextFactory;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(IDbContextFactory<ChatDbContext> contextFactory, ILogger<UserRepository> logger)
    {
        _contextFactory = contextFactory;
        _logger = logger;
    }

    public async Task RegisterAsync(User user)
    {
        using var context = _contextFactory.CreateDbContext();
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
    }
    public async Task<User?> GetByUsernameAsync(string username)
    {
        using var context = _contextFactory.CreateDbContext();
        _logger.LogInformation("Retrieving user by username: {Username}", username);
        return await context.Users.FirstOrDefaultAsync(u => u.Username == username);
    }
    public async Task SetStatus(Guid id,bool status)
    {
        using var context = _contextFactory.CreateDbContext();
        await context.Users.Where(u => u.UserID == id).ExecuteUpdateAsync(u => u.SetProperty(p => p.IsOnline, status));
        await context.SaveChangesAsync();
    }
    public async Task<User?> GetByIdAsync(Guid id)
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.Users.FirstOrDefaultAsync(u => u.UserID == id);
    }
    public async Task<HashSet<User>> GetMultipleUserByIdAsync(HashSet<Guid> ids)
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.Users
            .Where(u => ids.Contains(u.UserID))
            .ToHashSetAsync();
    }
    public async Task<List<User>> GetAllUsersToInviteAsync(Guid currentUserId, string query)
    {
        using var context = _contextFactory.CreateDbContext();
        _logger.LogInformation("Retrieving all users who can be invited with query: {Query}", query);
                var mutualContactIds = await context.Contacts
            .IgnoreQueryFilters()
            .Where(c => c.UserID == currentUserId && !c.IsDeleted)
            .Where(c => context.Contacts 
                .IgnoreQueryFilters()
                .Any(rc => rc.UserID == c.ContactUserID &&
                           rc.ContactUserID == currentUserId &&
                           !rc.IsDeleted))
            .Select(c => c.ContactUserID)
            .ToListAsync();

        var pendingInviteIds = await context.Invites
        .IgnoreQueryFilters()
        .Where(i => i.Status == InviteStatus.Pending &&
                   (i.SenderID == currentUserId || i.ReceiverID == currentUserId))
        .Select(i => i.SenderID == currentUserId ? i.ReceiverID : i.SenderID)
        .ToListAsync();

        return await context.Users
         .IgnoreQueryFilters()
         .Where(u => u.UserID != currentUserId)
         .Where(u => u.Username.Contains(query))
         .Where(u => !mutualContactIds.Contains(u.UserID))
         .Where(u => !pendingInviteIds.Contains(u.UserID))
         .ToListAsync();
    }
    public async Task SaveChangesToDbAsync()
    {
        using var context = _contextFactory.CreateDbContext();
        _logger.LogInformation("Saving changes to the database.");
        await context.SaveChangesAsync();
    }

}
