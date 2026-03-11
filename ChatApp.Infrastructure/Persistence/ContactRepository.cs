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
        public async Task<Contact> GetContactAsync(Guid contactId, Guid userId)
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
                .Where(c => c.UserID == id)
                .ToListAsync();
        }
        public async Task AddContactToDb(Contact contact)
        {
            using var context = _contextFactory.CreateDbContext();
            _logger.LogInformation("Adding a new contact to the database.");
            await context.Contacts.AddAsync(contact);
            await context.SaveChangesAsync();
        }
         public async Task SaveChangesToDbAsync()
        {
            using var context = _contextFactory.CreateDbContext();
            _logger.LogInformation("Saving changes to the database.");
            await context.SaveChangesAsync();
        }
    }
}
