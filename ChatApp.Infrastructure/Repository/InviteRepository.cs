using ChatApp.Domain.Enums;
using ChatApp.Domain.Interfaces.Repository;
using ChatApp.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ChatApp.Infrastructure.Persistence
{
    public class InviteRepository : IInviteRepository
    {
        private readonly ChatDbContext _context;
        private readonly ILogger<InviteRepository> _logger;

        public InviteRepository(ChatDbContext context, ILogger<InviteRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task AddInviteAsync(Guid senderId, Guid receiverId)
        {
            await _context.Invites.AddAsync(new Invite
            {
                SenderID = senderId,
                ReceiverID = receiverId,
                Status = InviteStatus.Pending,
            });
        }

        public async Task UpdateInviteStatusAsync(Invite invite)
        {
            _context.Invites.Update(invite);
            _logger.LogInformation("Invite status changed to {Status}", invite.Status);
        }

        public async Task<List<Invite>> GetInvitesForUserAsync(Guid userId)
        {
            return await _context.Invites
                .Include(i => i.Sender)
                .Include(i => i.Receiver)
                .Where(i => i.ReceiverID == userId && i.Status == InviteStatus.Pending)
                .ToListAsync();
        }

        public async Task<Invite?> GetInviteByIdAsync(Guid inviteId)
        {
            return await _context.Invites.FirstOrDefaultAsync(i => i.InviteID == inviteId);
        }
    }
}
