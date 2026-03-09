using ChatApp.Application.DTO;
using ChatApp.Application.Interfaces.Repository;
using ChatApp.Application.Interfaces.Service;
using ChatApp.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

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
        public async Task AddInviteToDB(Guid senderId, Guid receiverId)
        {
            _context.Invites.Add(new Invite
            {
                SenderID = senderId,
                ReceiverID = receiverId,
                Status = InviteStatus.Pending,
            });
        }
        public async Task SaveChangesToDbAsync()
        {
            _logger.LogInformation("Saving changes to the database.");
            await _context.SaveChangesAsync();
        }
        public async Task<List<InviteDTO>> GetInvitesForUserAsync(Guid userId)
        {
            return await _context.Invites
                .Where(i => i.ReceiverID == userId && i.Status == InviteStatus.Pending)
                .Select(i => new InviteDTO
                {
                    InviteID = i.InviteID,
                    SenderID = i.SenderID,
                    ReceiverID = i.ReceiverID,
                    SenderUsername = i.Sender.Username,
                    ReceiverUsername = i.Receiver.Username,
                    SenderAvatarUrl = i.Sender.AvatarUrl,
                    Status = i.Status,
                })
                .ToListAsync();
        }
        public async Task<Invite> GetInviteForIdAsync(Guid InviteId)
        {
            return await _context.Invites.FirstOrDefaultAsync(i => i.InviteID == InviteId);
        }
    }
}
