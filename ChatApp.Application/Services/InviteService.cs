using ChatApp.Application.DTO;
using ChatApp.Application.Interfaces.Repository;
using ChatApp.Application.Interfaces.Service;
using ChatApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

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
        public async Task SendInvite(Guid senderId, Guid receiverId)
        {
            await _inviteRepo.AddInviteToDB(senderId, receiverId);
  
        }
        public async Task<List<InviteDTO>> GetInvites(Guid userId)
        {
            var invites = await _inviteRepo.GetInvitesForUserAsync(userId);
            return  invites.Select(i => new InviteDTO
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
        public async Task<Guid> InviteAction(Guid InviteId, bool status,Guid userId)
        {
            var invite = await _inviteRepo.GetInviteForIdAsync(InviteId);
            if (invite.ReceiverID != userId)
            {
                return invite.SenderID;
            }
            if (invite != null && status)
            {
                await _contactService.AddContactAsync(invite.SenderID, invite.ReceiverID);
                invite.Status = InviteStatus.Accepted;
                await _inviteRepo.ChangeInviteStatus(invite);
                
               
            }
            else
                if (invite != null && !status)
                {
                    invite.Status = InviteStatus.Rejected;
                    await _inviteRepo.ChangeInviteStatus(invite);
                }
            return invite.SenderID;
        }
    }
}
