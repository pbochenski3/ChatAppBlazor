using ChatApp.Application.DTO;
using ChatApp.Application.Interfaces.Repository;
using ChatApp.Application.Interfaces.Service;
using ChatApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Services
{
    public class ContactService : IContactService
    {
        private readonly IMessageRepository _messageRepo;
        private readonly IUserRepository _userRepo;
        private readonly IContactRepository _contactRepo;

        public ContactService(IMessageRepository messageRepo, IUserRepository userRepo, IContactRepository contactRepo)
        {
            _messageRepo = messageRepo;
            _userRepo = userRepo;
            _contactRepo = contactRepo;
        }
        public async Task<List<ContactDTO>> GetUserContactsAsync(Guid id)
        {
            var contacts = await _contactRepo.GetAllContactAsync(id);

            if (contacts == null) return new List<ContactDTO>();
            return contacts.Select(c => new ContactDTO
            {
                ContactUserID = c.ContactUserID,
                Username = c.ContactUser.Username,
                AvatarUrl = c.ContactUser.AvatarUrl,
                IsOnline = c.ContactUser.IsOnline,
                // ChatID = await _messageRepo.GetChatIdByUsersAsync(id, c.UserID)
            }).ToList();
        }
        public async Task AddContactAsync(Guid userId, Guid contactUserId)
        {
            var contact1 = new Contact
            {
                UserID = userId,
                ContactUserID = contactUserId
            };
            var contact2 = new Contact
            {
                UserID = contactUserId,
                ContactUserID = userId
            };
            await _contactRepo.AddContactToDb(contact1);
            await _contactRepo.AddContactToDb(contact2);
            await _contactRepo.SaveChangesToDbAsync();
        }
    }
}
