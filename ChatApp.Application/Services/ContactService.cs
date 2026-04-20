using ChatApp.Application.DTO;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Interfaces.Repository;
using ChatApp.Application.Interfaces.Service;
using ChatApp.Application.Notifications.Contact;
using ChatApp.Domain.Models;
using ChatApp.Domain.Repository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace ChatApp.Application.Services
{
    public class ContactService : IContactService
    {
        private readonly IMessageRepository _messageRepo;
        private readonly IUserRepository _userRepo;
        private readonly IContactRepository _contactRepo;
        private readonly IChatRepository _chatRepo;
        private readonly IUserChatRepository _userChatRepo;
        private readonly ITransactionProvider _transactionProvider;
        private readonly IMediator _mediator;

        public ContactService(
            IContactRepository contactRepo,
            IMessageRepository messageRepo,
            IUserRepository userRepo, IChatRepository chatRepo,
            IUserChatRepository userChatRepo,
            ITransactionProvider transactionProvider,
            IMediator mediator
            )
        {
            _messageRepo = messageRepo;
            _userRepo = userRepo;
            _chatRepo = chatRepo;
            _contactRepo = contactRepo;
            _transactionProvider = transactionProvider;
            _userChatRepo = userChatRepo;
            _mediator = mediator;
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
            var contacts = await _contactRepo.GetAllContactsAsync(userId);
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
             await _transactionProvider.ExecuteInTransactionAsync(async () =>
            {
                var contact1 = new Contact
                {
                    UserID = userId1,
                    ContactUserID = userId2,
                };
                await _contactRepo.AddContactAsync(contact1);

                var contact2 = new Contact
                {
                    UserID = userId2,
                    ContactUserID = userId1
                };
                await _contactRepo.AddContactAsync(contact2);
            });
        }

        public async Task AddContactAsync(Guid userId, Guid contactUserId)
        {
            if (userId == Guid.Empty || contactUserId == Guid.Empty)
            {
                throw new ArgumentException("[AddContactAsync] userId or contactUserId is empty");
            }

            var isDeleted = await _contactRepo.IsContactDeletedAsync(userId, contactUserId);
            if (!isDeleted)
            {
                await CreateContactAsync(userId, contactUserId);
            }
            else
            {
                await _contactRepo.RestoreContactsAsync(userId, contactUserId);
            }
        }

        public async Task RemoveContactAsync(Guid contactUserId, Guid userId, Guid chatId)
        {
            await _transactionProvider.ExecuteInTransactionAsync(async () =>
            {
            await _contactRepo.DeleteContactAsync(contactUserId, userId);
            await _userChatRepo.ArchivePrivateChatAsync(chatId, userId, contactUserId);
            await _mediator.Publish(new ContactDeletedNotification(contactUserId,userId,chatId));
            });
        }
    }
}
