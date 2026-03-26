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
    public class InviteService : IInviteService
    {
        private readonly IContactService _contactService;
        private readonly IInviteRepository _inviteRepo;

        public InviteService(IInviteRepository inviteRepo, IContactService contactService)
        {
            _inviteRepo = inviteRepo;
            _contactService = contactService;
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

        public async Task<Guid> HandleInviteActionAsync(Guid inviteId, bool status, Guid userId)
        {
            var invite = await _inviteRepo.GetInviteByIdAsync(inviteId);
            if (invite == null || invite.ReceiverID != userId)
            {
                return invite?.SenderID ?? Guid.Empty;
            }

            if (status)
            {
                await _contactService.AddContactAsync(invite.SenderID, invite.ReceiverID);
                invite.Status = InviteStatus.Accepted;
                await _inviteRepo.UpdateInviteStatusAsync(invite);
            }
            else
            {
                invite.Status = InviteStatus.Rejected;
                await _inviteRepo.UpdateInviteStatusAsync(invite);
            }

            return invite.SenderID;
        }
    }
}
