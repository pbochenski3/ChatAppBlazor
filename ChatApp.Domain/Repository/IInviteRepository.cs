using ChatApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatApp.Application.Interfaces.Repository
{
    public interface IInviteRepository
    {
        Task SaveChangesToDbAsync();
        Task AddInviteAsync(Guid senderId, Guid receiverId);
        Task<List<Invite>> GetInvitesForUserAsync(Guid userId);
        Task<Invite?> GetInviteByIdAsync(Guid inviteId);
        Task UpdateInviteStatusAsync(Invite invite);
    }
}
