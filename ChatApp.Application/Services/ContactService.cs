using ChatApp.Application.DTO;
using ChatApp.Application.Interfaces.Repository;
using ChatApp.Application.Interfaces.Service;
using ChatApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Application.Services
{
    public class ContactService : IContactService
    {
        private readonly IMessageRepository _messageRepo;
        private readonly IUserRepository _userRepo;
        private readonly IContactRepository _contactRepo;
        private readonly IChatRepository _chatRepo;

        public ContactService(IContactRepository contactRepo, IMessageRepository messageRepo, IUserRepository userRepo, IChatRepository chatRepo)
        {
            _messageRepo = messageRepo;
            _userRepo = userRepo;
            _chatRepo = chatRepo;
            _contactRepo = contactRepo;
        }

        public async Task<ContactDTO?> GetContactByIdAsync(Guid contactUserId, Guid userId)
        {
            var contact = await _contactRepo.GetContactAsync(contactUserId, userId);
            if (contact == null)
            {
                return null;
            }
            return new ContactDTO
            {
                ContactUserID = contact.ContactUserID,
                Username = contact.ContactUser.Username,
                AvatarUrl = contact.ContactUser.AvatarUrl,
                IsOnline = contact.ContactUser.IsOnline,
            };
        }

        public async Task<List<ContactDTO>> GetUserContactsAsync(Guid userId)
        {
            var contacts = await _contactRepo.GetAllContactAsync(userId);
            if (contacts == null || !contacts.Any())
            {
                return new List<ContactDTO>();
            }

            return contacts.Select(c => new ContactDTO
            {
                ContactUserID = c.ContactUserID,
                Username = c.ContactUser.Username,
                AvatarUrl = c.ContactUser.AvatarUrl,
                IsOnline = c.ContactUser.IsOnline,
            }).ToList();
        }

        public async Task CreateContactAsync(Guid userId1, Guid userId2)
        {
            var contact1 = new Contact
            {
                UserID = userId1,
                ContactUserID = userId2,
            };
            await _contactRepo.AddContactToDb(contact1);

            var contact2 = new Contact
            {
                UserID = userId2,
                ContactUserID = userId1
            };
            await _contactRepo.AddContactToDb(contact2);
        }

        public async Task AddContactAsync(Guid userId, Guid contactUserId)
        {
            if (userId == Guid.Empty || contactUserId == Guid.Empty)
            {
                throw new ArgumentException("[AddContactAsync] userId or contactUserId is empty");
            }

            var isDeleted = await _contactRepo.CheckDeletedContact(userId, contactUserId);
            if (!isDeleted)
            {
                await CreateContactAsync(userId, contactUserId);
            }
            else
            {
                await _contactRepo.RestoreContacts(userId, contactUserId);
            }
        }

        public async Task RemoveContactAsync(Guid contactUserId, Guid userId, Guid chatId)
        {
            await _contactRepo.DeleteContactFromDb(contactUserId, userId);
            await _chatRepo.ArchivePrivateChatAsync(chatId, userId, contactUserId);
        }
    }
}
