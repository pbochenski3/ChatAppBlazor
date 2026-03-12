using ChatApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Interfaces.Repository
{
    public interface IContactRepository
    {
        Task<List<Contact?>> GetAllContactAsync(Guid id);
        Task AddContactToDb(Contact contact);
        Task<Contact> GetContactAsync(Guid contactId, Guid userId);
        Task DeleteContactFromDb(Guid contactId, Guid userId);
        Task<Contact?> CheckContact(Guid contact1, Guid contact2);
        Task RestoreContacts(List<Contact> contactRestore);

    }
}
