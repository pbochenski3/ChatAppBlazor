using ChatApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Interfaces.Repository
{
    public interface IContactRepository
    {
        Task<List<Contact>> GetAllContactAsync(Guid id);
        Task AddContactToDb(Contact contact);
        Task SaveChangesToDbAsync();
        Task<Contact> GetContactAsync(Guid contactId, Guid userId);
    }
}
