using ChatApp.Application.DTO;
using ChatApp.Application.Interfaces.Repository;
using ChatApp.Application.Interfaces.Service;
using ChatApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChatApp.Application.Services
{
    public class ContactService : IContactService
    {
        private readonly IMessageRepository _messageRepo;
        private readonly IUserRepository _userRepo;
        private readonly IContactRepository _contactRepo;
        private readonly IChatRepository _chattRepo;

        public ContactService(IContactRepository contactRepo, IMessageRepository messageRepo, IUserRepository userRepo, IChatRepository chatRepo)
        {
            _messageRepo = messageRepo;
            _userRepo = userRepo;
            _chattRepo = chatRepo;
            _contactRepo = contactRepo;
        }
        public async Task<ContactDTO> GetContactById(Guid contactId, Guid userId)
        {
            var contact = await _contactRepo.GetContactAsync(contactId, userId);
            if (contact == null)
            {
                throw new Exception("Contact couldn`t load");
            }
            return new ContactDTO
            {
                ContactUserID = contact.ContactUserID,
                Username = contact.ContactUser.Username,
                AvatarUrl = contact.ContactUser.AvatarUrl,
                IsOnline = contact.ContactUser.IsOnline,
            };
        }
        public async Task<List<ContactDTO>> GetUserContactsAsync(Guid id)
        {
            var contacts = await _contactRepo.GetAllContactAsync(id);

            if (contacts == null || !contacts.Any())
            {
                return new List<ContactDTO>();
            }

            var result = new List<ContactDTO>();

            foreach (var c in contacts.Where(c => c.ContactUser != null))
            {
                var dto = new ContactDTO
                {
                    ContactUserID = c.ContactUserID,
                    Username = c.ContactUser.Username,
                    AvatarUrl = c.ContactUser.AvatarUrl,
                    IsOnline = c.ContactUser.IsOnline,
                };


                result.Add(dto);
            }

            return result;
        }
    
        public async Task CreateContact(Guid contact1, Guid contact2)
        {
            var contact = new Contact
            {
                UserID = contact1,
                ContactUserID = contact2,
            };
            await _contactRepo.AddContactToDb(contact);
            contact = new Contact
            {
                UserID = contact2,
                ContactUserID = contact1
            };
            await _contactRepo.AddContactToDb(contact);
        }
        public async Task AddContactAsync(Guid userId, Guid contactUserId)
        {
            var contact1 = await _contactRepo.CheckContact(userId,contactUserId);
            var contact2 = await _contactRepo.CheckContact(contactUserId,userId);
            if(contact1 == null && contact2 == null )
            {
                await CreateContact(userId,contactUserId);
            }
            else
            { 
            var toRestore = new List<Contact>();

            if (contact1 != null && contact1.IsDeleted) toRestore.Add(contact1);
            if (contact2 != null && contact2.IsDeleted) toRestore.Add(contact2);

            if (toRestore.Any())
            {
                await _contactRepo.RestoreContacts(toRestore);
            }
        }
}
        public async Task DeleteContactAsync(Guid contactId,Guid userId,Guid chatId)
        {
            await _contactRepo.DeleteContactFromDb(contactId, userId);
            await _chattRepo.ArchivePrivateChatFromDb(chatId,userId,contactId);
        }
    }
}
