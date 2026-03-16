using ChatApp.Application.Interfaces.Repository;
using ChatApp.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Infrastructure.Persistence
{
    public class ContactRepository : IContactRepository
    {
        private readonly IDbContextFactory<ChatDbContext> _contextFactory;
        private readonly ILogger<MessageRepository> _logger;

        public ContactRepository(IDbContextFactory<ChatDbContext> contextFactory, ILogger<MessageRepository> logger)
        {
            _contextFactory = contextFactory;
            _logger = logger;
        }
        public async Task<Contact?> GetContactAsync(Guid contactId, Guid userId)
        {
            using var context = _contextFactory.CreateDbContext();
            _logger.LogInformation("Retreiving contact from database");
            return await context.Contacts
                .Include(c => c.ContactUser)
                .FirstOrDefaultAsync(c => c.ContactUserID == contactId && c.UserID == userId);
        }
        public async Task<List<Contact>> GetAllContactAsync(Guid id)
        {
            using var context = _contextFactory.CreateDbContext();
            _logger.LogInformation("Retrieving all user contact from the database.");
            return await context.Contacts
                .Include(c => c.ContactUser)
                .Where(c => c.UserID == id && c.IsDeleted == false)
                .ToListAsync();
        }
        public async Task<Contact?> CheckContact(Guid contact1, Guid contact2)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Contacts
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(c =>
                c.UserID == contact1 &&
                c.ContactUserID == contact2 &&
                c.IsDeleted == true);

        }
        public async Task RestoreContacts(List<Contact> contactRestore)
        {
            using var context = _contextFactory.CreateDbContext();

            if (contactRestore == null || !contactRestore.Any()) return;

            foreach (var contact in contactRestore)
            {
                await context.Contacts
                    .IgnoreQueryFilters()
                    .Where(c => c.UserID == contact.UserID && c.ContactUserID == contact.ContactUserID)
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(c => c.IsDeleted, false)
                        .SetProperty(c => c.DeletedAt, (DateTime?)null));
            }

        }
        public async Task AddContactToDb(Contact contact)
        {
            using var context = _contextFactory.CreateDbContext();
            _logger.LogInformation("Adding a new contact to the database.");
            await context.Contacts.AddAsync(contact);
            await context.SaveChangesAsync();
        }
        public async Task DeleteContactFromDb(Guid contactId, Guid userId)
        {
            using var context = _contextFactory.CreateDbContext();

            await context.Contacts
                .IgnoreQueryFilters()
                .Where(c => c.UserID == userId && c.ContactUserID == contactId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(c => c.IsDeleted, true)
                    .SetProperty(c => c.DeletedAt, DateTime.UtcNow));
            await context.Contacts
                .IgnoreQueryFilters()
                .Where(c => c.UserID == contactId && c.ContactUserID == userId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(c => c.IsDeleted, true)
                    .SetProperty(c => c.DeletedAt, DateTime.UtcNow));

            await context.SaveChangesAsync();
        }

    }
}
