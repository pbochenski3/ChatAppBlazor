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
            await _inviteRepo.SaveChangesToDbAsync();
  
        }
        public async Task<List<InviteDTO>> GetInvites(Guid userId)
        {
            return await _inviteRepo.GetInvitesForUserAsync(userId);
        }
        public async Task<Guid> InviteAction(Guid InviteId, bool status)
        {
            var invite = await _inviteRepo.GetInviteForIdAsync(InviteId);
            if (invite != null && status)
            {
                await _contactService.AddContactAsync(invite.SenderID, invite.ReceiverID);
                invite.Status = InviteStatus.Accepted;
               
            }
            else
                if (invite != null && !status)
                {
                    invite.Status = InviteStatus.Rejected;
                }
            await _inviteRepo.SaveChangesToDbAsync();
            return invite.SenderID;
        }
    }
}
