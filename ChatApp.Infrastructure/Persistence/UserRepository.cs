using ChatApp.Domain.Enums;
using ChatApp.Domain.Interfaces.Repository;
using ChatApp.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ChatApp.Infrastructure.Persistence
{
    public class UserRepository : IUserRepository
    {
        private readonly ChatDbContext _context;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(ChatDbContext context, ILogger<UserRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task RegisterAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }
        public async Task<string> GetAvatarUrlAsync(Guid userId)
        {
            var avatarUrl = await _context.Users
                .Where(u => u.UserID == userId)
                .Select(u => u.AvatarUrl)
                .FirstOrDefaultAsync();
            if (string.IsNullOrEmpty(avatarUrl))
            {
                _logger.LogWarning("User with ID {UserId} does not have an avatar URL.", userId);
                return string.Empty;
            }
            return avatarUrl;
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            _logger.LogInformation("Retrieving user by username: {Username}", username);
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task SetUserStatusAsync(Guid id, bool status)
        {
            await _context.Users
                .Where(u => u.UserID == id)
                .ExecuteUpdateAsync(u => u.SetProperty(p => p.IsOnline, status));
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserID == id);
        }

        public async Task<List<User>> GetUsersByIdsAsync(HashSet<Guid> ids)
        {
            var users = await _context.Users
                .Where(u => ids.Contains(u.UserID))
                .ToListAsync();
            return users.ToList();
        }

        public async Task<List<User>> GetAllUsersToInviteAsync(Guid currentUserId, string query)
        {
            _logger.LogInformation("Searching users to invite for: {CurrentUserId}, Query: {Query}", currentUserId, query);

            var usersQuery = _context.Users
                .IgnoreQueryFilters()
                .Where(u => u.UserID != currentUserId && u.Username.Contains(query));

            usersQuery = usersQuery.Where(u => !_context.Contacts
                .Any(c => c.UserID == currentUserId &&
                          c.ContactUserID == u.UserID &&
                          !c.IsDeleted &&
                          _context.Contacts.Any(rc => rc.UserID == u.UserID &&
                                                      rc.ContactUserID == currentUserId &&
                                                      !rc.IsDeleted)));

            usersQuery = usersQuery.Where(u => !_context.Invites
                .Any(i => i.Status == InviteStatus.Pending &&
                          (i.SenderID == u.UserID || i.ReceiverID == u.UserID) &&
                          (i.SenderID == currentUserId || i.ReceiverID == currentUserId)));

            return await usersQuery
            .ToListAsync();
        }
        public async Task UpdateAvatarAsync(Guid userId, string avatarUrl)
        {
            await _context.Users
                .Where(u => u.UserID == userId)
                .ExecuteUpdateAsync(u => u.SetProperty(p => p.AvatarUrl, avatarUrl));
        }
    }
}
