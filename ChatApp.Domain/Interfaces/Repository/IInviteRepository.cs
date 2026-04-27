using ChatApp.Domain.Models;

namespace ChatApp.Domain.Interfaces.Repository
{
    public interface IInviteRepository
    {
        Task AddInviteAsync(Guid senderId, Guid receiverId);
        Task<List<Invite>> GetInvitesForUserAsync(Guid userId);
        Task<Invite?> GetInviteByIdAsync(Guid inviteId);
        Task UpdateInviteStatusAsync(Invite invite);
    }
}
