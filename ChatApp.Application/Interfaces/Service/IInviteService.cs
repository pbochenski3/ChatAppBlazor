using ChatApp.Application.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Interfaces.Service
{
    public interface IInviteService
    {
        Task<Guid> InviteAction(Guid InviteId, bool status);
        Task<List<InviteDTO>> GetInvites(Guid userId);
        Task SendInvite(Guid senderId, Guid receiverId);
    }
}
