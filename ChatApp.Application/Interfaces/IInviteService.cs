using ChatApp.Application.DTO;
using ChatApp.Application.DTO.Requests;
using ChatApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatApp.Application.Interfaces.Service
{
    public interface IInviteService
    {
        Task UpdateInviteStatusAsync(Guid inviteId, InviteStatus status);
        Task<List<InviteDTO>> GetUserInvitesAsync(Guid userId);
        Task SendInviteAsync(Guid senderId, Guid receiverId);
        Task ProcessInviteActionAsync(InviteActionRequest request, CancellationToken ct);
    }
}
