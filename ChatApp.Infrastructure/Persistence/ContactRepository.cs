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
        private readonly ChatDbContext _context;
        private readonly ILogger<MessageRepository> _logger;

        public ContactRepository(ChatDbContext context, ILogger<MessageRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<List<Contact>> GetAllContactAsync(Guid id)
        {
            _logger.LogInformation("Retrieving all user contact from the database.");
            return await _context.Contacts
                .Include(c => c.ContactUser)
                .Where(c => c.UserID == id)
                .ToListAsync();
        }
        public async Task AddContactToDb(Contact contact)
        {
            _logger.LogInformation("Adding a new contact to the database.");
            await _context.Contacts.AddAsync(contact);
        }
         public async Task SaveChangesToDbAsync()
        {
            _logger.LogInformation("Saving changes to the database.");
            await _context.SaveChangesAsync();
        }
    }
}
