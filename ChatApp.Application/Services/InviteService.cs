using ChatApp.Application.DTO;
using ChatApp.Application.DTO.Requests;
using ChatApp.Application.DTO.Results;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Interfaces.Chats;
using ChatApp.Application.Interfaces.Repository;
using ChatApp.Application.Interfaces.Service;
using ChatApp.Domain.Enums;
using ChatApp.Domain.Models;
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

        public InviteService(IInviteRepository inviteRepo, IContactService contactService, IPrivateChatService privateChatService, ITransactionProvider transactionProvider)
        {
            _inviteRepo = inviteRepo;
            _contactService = contactService;
            _privateChatService = privateChatService;
            _transactionProvider = transactionProvider;
        }

        public async Task SendInviteAsync(Guid senderId, Guid receiverId)
        {
            await _inviteRepo.AddInviteAsync(senderId, receiverId);
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
        public async Task<InviteActionResultDto> ProcessInviteActionAsync(Guid inviteId, InviteStatus response, CancellationToken ct)
        {
            var invite = await _inviteRepo.GetInviteByIdAsync(inviteId);

            if (invite == null)
            {
                throw new KeyNotFoundException("Invite not found");
            }

            return await _transactionProvider.ExecuteAsync(async () =>
            {
                Guid chatId = Guid.Empty;

                if (response == InviteStatus.Accepted)
                {
                    await _contactService.AddContactAsync(invite.SenderID, invite.ReceiverID);
                    chatId = await _privateChatService.CreatePrivateChatAsync(invite.SenderID, invite.ReceiverID);
                }
                return new InviteActionResultDto(invite.SenderID, invite.ReceiverID, chatId);
            });
        }
    }
}
