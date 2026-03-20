using ChatApp.Application.Interfaces.Repository;
using ChatApp.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
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
        public async Task<bool> CheckDeletedContact(Guid userId, Guid contactUserId)
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
        public async Task RestoreContacts(Guid userId,Guid contactUserId)
        {
            using var context = _contextFactory.CreateDbContext();

            if(userId == Guid.Empty || contactUserId == Guid.Empty) { return; }

                await context.Contacts
                    .IgnoreQueryFilters()
                    .Where(c => c.UserID == userId && c.ContactUserID == contactUserId
                    || c.UserID == contactUserId && c.ContactUserID == userId)
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(c => c.IsDeleted, false)
                        .SetProperty(c => c.DeletedAt, (DateTime?)null));

        }
        public async Task AddContactToDb(Contact contact)
        {
            using var context = _contextFactory.CreateDbContext();
            _logger.LogInformation("Adding a new contact to the database.");
            await context.Contacts.AddAsync(contact);
            await context.SaveChangesAsync();
        }
        public async Task<bool> CheckIfContact(Guid userId, Guid contactId,CancellationToken token)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Contacts
                .AnyAsync(c => c.UserID == userId && c.ContactUserID == contactId,token);
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
