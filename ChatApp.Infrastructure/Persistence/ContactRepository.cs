using ChatApp.Application.Interfaces.Repository;
using ChatApp.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ChatApp.Infrastructure.Persistence
{
    public class ContactRepository : IContactRepository
    {
        private readonly ChatDbContext _context;
        private readonly ILogger<ContactRepository> _logger;

        public ContactRepository(ChatDbContext context, ILogger<ContactRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Contact?> GetContactAsync(Guid contactUserId, Guid userId)
        {
            _logger.LogInformation("Retrieving contact from database");
            return await _context.Contacts
                .Include(c => c.ContactUser)
                .FirstOrDefaultAsync(c => c.ContactUserID == contactUserId && c.UserID == userId);
        }

        public async Task<List<Contact>> GetAllContactsAsync(Guid userId)
        {
            _logger.LogInformation("Retrieving all user contacts from the database for user {UserId}", userId);
            return await _context.Contacts
                .Include(c => c.ContactUser)
                .Where(c => c.UserID == userId && c.IsDeleted == false)
                .ToListAsync();
        }

        public async Task<bool> IsContactDeletedAsync(Guid userId, Guid contactUserId)
        {
            return await _context.Contacts
                .IgnoreQueryFilters()
                .AnyAsync(c =>
                    c.IsDeleted == true && (
                        (c.UserID == userId && c.ContactUserID == contactUserId) ||
                        (c.UserID == contactUserId && c.ContactUserID == userId)
                    ));
        }

        public async Task RestoreContactsAsync(Guid userId, Guid contactUserId)
        {

            if (userId == Guid.Empty || contactUserId == Guid.Empty) return;

            await _context.Contacts
                .IgnoreQueryFilters()
                .Where(c => (c.UserID == userId && c.ContactUserID == contactUserId)
                         || (c.UserID == contactUserId && c.ContactUserID == userId))
                .ExecuteUpdateAsync(s => s
                    .SetProperty(c => c.IsDeleted, false)
                    .SetProperty(c => c.DeletedAt, (DateTime?)null));
        }

        public async Task AddContactAsync(Contact contact)
        {
            _logger.LogInformation("Adding a new contact to the database.");
            await _context.Contacts.AddAsync(contact);
        }

        public async Task<bool> IsContactExistingAsync(Guid userId, Guid contactId, CancellationToken token)
        {
            return await _context.Contacts
                .AnyAsync(c => c.UserID == userId && c.ContactUserID == contactId, token);
        }

        public async Task DeleteContactAsync(Guid contactUserId, Guid userId)
        {

            await _context.Contacts
                .IgnoreQueryFilters()
                .Where(c => (c.UserID == userId && c.ContactUserID == contactUserId)
                         || (c.UserID == contactUserId && c.ContactUserID == userId))
                .ExecuteUpdateAsync(s => s
                    .SetProperty(c => c.IsDeleted, true)
                    .SetProperty(c => c.DeletedAt, DateTime.UtcNow));
        }
    }
}
