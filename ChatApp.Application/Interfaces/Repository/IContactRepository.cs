using ChatApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Interfaces.Repository
{
    public interface IContactRepository
    {
        public Task<List<Contact>> GetAllContactAsync(Guid id);
        public Task AddContactToDb(Contact contact);
        public Task SaveChangesToDbAsync();
    }
}
