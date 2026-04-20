using ChatApp.Application.DTO;
using ChatApp.Application.DTO.Requests;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Interfaces.Chats;
using ChatApp.Application.Interfaces.Repository;
using ChatApp.Application.Interfaces.Service;
using ChatApp.Application.Notifications.Invite;
using ChatApp.Domain.Enums;
using ChatApp.Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Application.Services
{
    public class InviteService : IInviteService
    {
        private readonly ITransactionProvider _transactionProvider;
        private readonly IContactService _contactService;
        private readonly IInviteRepository _inviteRepo;
        private readonly IPrivateChatService _privateChatService;
        private readonly IMediator _mediator;

        public InviteService(IInviteRepository inviteRepo, IContactService contactService, IPrivateChatService privateChatService, ITransactionProvider transactionProvider,IMediator mediator)
        {
            _inviteRepo = inviteRepo;
            _contactService = contactService;
            _privateChatService = privateChatService;
            _transactionProvider = transactionProvider;
            _mediator = mediator;
        }

        public async Task SendInviteAsync(Guid senderId, Guid receiverId)
        {
            await _inviteRepo.AddInviteAsync(senderId, receiverId);
            await _mediator.Publish(new ContactInviteSendedNotification(senderId, receiverId));

        }

        public async Task<List<InviteDTO>> GetUserInvitesAsync(Guid userId)
        {
            var invites = await _inviteRepo.GetInvitesForUserAsync(userId);
            return invites.Select(i => new InviteDTO
            {
                InviteID = i.InviteID,
                SenderID = i.SenderID,
                ReceiverID = i.ReceiverID,
                SenderUsername = i.Sender.Username,
                ReceiverUsername = i.Receiver.Username,
                SenderAvatarUrl = i.Sender.AvatarUrl,
                Status = i.Status,
            }).ToList();
        }
        public async Task UpdateInviteStatusAsync(Guid inviteId, InviteStatus status)
        {
            var invite = await _inviteRepo.GetInviteByIdAsync(inviteId);
            if (invite == null)
            {
                throw new KeyNotFoundException("Invite not found");
            }
            invite.Status = status;
            await _inviteRepo.UpdateInviteStatusAsync(invite);
        }
        public async Task ProcessInviteActionAsync(InviteActionRequest request, CancellationToken ct)
        {
            await UpdateInviteStatusAsync(request.InviteId, request.Response);
            var invite = await _inviteRepo.GetInviteByIdAsync(request.InviteId);

            if (invite == null)
            {
                throw new KeyNotFoundException("Invite not found");
            }

            await _transactionProvider.ExecuteInTransactionAsync(async () =>
            {
                Guid newChatId = Guid.Empty;

                if (request.Response == InviteStatus.Accepted)
                {
                    await _contactService.AddContactAsync(invite.SenderID, invite.ReceiverID);
                    newChatId = await _privateChatService.CreatePrivateChatAsync(invite.SenderID, invite.ReceiverID);
                }
                await _mediator.Publish(new InviteActionNotification(invite.SenderID,invite.ReceiverID,request.chatId,newChatId,request.Response));

            });
        }
    }
}
