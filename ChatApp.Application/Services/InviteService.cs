using ChatApp.Application.Interfaces.Repository;
using ChatApp.Application.Interfaces.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Services
{
    public class InviteService : IInviteService
    {
        private readonly IInviteRepository _inviteRepo;
        public InviteService(IInviteRepository inviteRepo)
        {
            _inviteRepo = inviteRepo;
        }
        public async Task SendInvite(Guid senderId, Guid receiverId)
        {
            await _inviteRepo.AddInviteToDB(senderId, receiverId);
            await _inviteRepo.SaveChangesToDbAsync();
        }
    }
}
