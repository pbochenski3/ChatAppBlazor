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
        Task<bool> CheckDeletedContact(Guid contact1, Guid contact2);
        Task RestoreContacts(Guid userId, Guid contactUserId);
        Task<bool> CheckIfContact(Guid userId, Guid contactId,CancellationToken token);

    }
}
