using ChatApp.Application.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatApp.Application.Interfaces.Service
{
    public interface IInviteService
    {
        Task<Guid> HandleInviteActionAsync(Guid inviteId, bool status, Guid userId);
        Task<List<InviteDTO>> GetUserInvitesAsync(Guid userId);
        Task SendInviteAsync(Guid senderId, Guid receiverId);
    }
}
