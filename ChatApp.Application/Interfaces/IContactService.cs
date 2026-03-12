using ChatApp.Application.DTO;
using ChatApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Interfaces.Service
{
    public interface IContactService
    {
        Task<List<ContactDTO>> GetUserContactsAsync(Guid id);
        Task AddContactAsync(Guid userId, Guid contactUserId);
        Task<ContactDTO> GetContactById(Guid contactId, Guid userId);
        Task DeleteContactAsync(Guid contactId, Guid userId, Guid chatId);
        Task CreateContact(Guid contact1, Guid contact2);
    }
}
