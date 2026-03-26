using ChatApp.Application.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatApp.Application.Interfaces.Service
{
    public interface IContactService
    {
        Task<List<ContactDTO>> GetUserContactsAsync(Guid userId);
        Task AddContactAsync(Guid userId, Guid contactUserId);
        Task<ContactDTO?> GetContactByIdAsync(Guid contactUserId, Guid userId);
        Task RemoveContactAsync(Guid contactUserId, Guid userId, Guid chatId);
        Task CreateContactAsync(Guid userId1, Guid userId2);
    }
}
