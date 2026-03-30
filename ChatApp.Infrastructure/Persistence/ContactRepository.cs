using ChatApp.Application.Interfaces.Repository;
using ChatApp.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChatApp.Infrastructure.Persistence
{
    public class ContactRepository : IContactRepository
    {
        private readonly IDbContextFactory<ChatDbContext> _contextFactory;
        private readonly ILogger<ContactRepository> _logger;

        public ContactRepository(IDbContextFactory<ChatDbContext> contextFactory, ILogger<ContactRepository> logger)
        {
            _contextFactory = contextFactory;
            _logger = logger;
        }

        public async Task<Contact?> GetContactAsync(Guid contactUserId, Guid userId)
        {
            using var context = _contextFactory.CreateDbContext();
            _logger.LogInformation("Retrieving contact from database");
            return await context.Contacts
                .Include(c => c.ContactUser)
                .FirstOrDefaultAsync(c => c.ContactUserID == contactUserId && c.UserID == userId);
        }

        public async Task<List<Contact>> GetAllContactsAsync(Guid userId)
        {
            using var context = _contextFactory.CreateDbContext();
            _logger.LogInformation("Retrieving all user contacts from the database for user {UserId}", userId);
            return await context.Contacts
                .Include(c => c.ContactUser)
                .Where(c => c.UserID == userId && c.IsDeleted == false)
                .ToListAsync();
        }

        public async Task<bool> IsContactDeletedAsync(Guid userId, Guid contactUserId)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Contacts
                .IgnoreQueryFilters()
                .AnyAsync(c =>
                    c.IsDeleted == true && (
                        (c.UserID == userId && c.ContactUserID == contactUserId) ||
                        (c.UserID == contactUserId && c.ContactUserID == userId)
                    ));
        }

        public async Task RestoreContactsAsync(Guid userId, Guid contactUserId)
        {
            using var context = _contextFactory.CreateDbContext();

            if (userId == Guid.Empty || contactUserId == Guid.Empty) return;

            await context.Contacts
                .IgnoreQueryFilters()
                .Where(c => (c.UserID == userId && c.ContactUserID == contactUserId)
                         || (c.UserID == contactUserId && c.ContactUserID == userId))
                .ExecuteUpdateAsync(s => s
                    .SetProperty(c => c.IsDeleted, false)
                    .SetProperty(c => c.DeletedAt, (DateTime?)null));
        }

        public async Task AddContactAsync(Contact contact)
        {
            using var context = _contextFactory.CreateDbContext();
            _logger.LogInformation("Adding a new contact to the database.");
            await context.Contacts.AddAsync(contact);
            await context.SaveChangesAsync();
        }

        public async Task<bool> IsContactExistingAsync(Guid userId, Guid contactId, CancellationToken token)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Contacts
                .AnyAsync(c => c.UserID == userId && c.ContactUserID == contactId, token);
        }

        public async Task DeleteContactAsync(Guid contactUserId, Guid userId)
        {
            using var context = _contextFactory.CreateDbContext();

            await context.Contacts
                .IgnoreQueryFilters()
                .Where(c => (c.UserID == userId && c.ContactUserID == contactUserId)
                         || (c.UserID == contactUserId && c.ContactUserID == userId))
                .ExecuteUpdateAsync(s => s
                    .SetProperty(c => c.IsDeleted, true)
                    .SetProperty(c => c.DeletedAt, DateTime.UtcNow));
        }
    }
}
