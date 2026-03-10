using ChatApp.Application.DTO;
using ChatApp.Application.Interfaces.Repository;
using ChatApp.Application.Interfaces.Service;
using ChatApp.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Infrastructure.Persistence
{
    public class InviteRepository : IInviteRepository
    {
        private readonly IDbContextFactory<ChatDbContext> _contextFactory;
        private readonly ILogger<InviteRepository> _logger;
        public InviteRepository(IDbContextFactory<ChatDbContext> contextFactory, ILogger<InviteRepository> logger) 
        {
            _contextFactory = contextFactory;
            _logger = logger;
        }
        public async Task AddInviteToDB(Guid senderId, Guid receiverId)
        {
            using var context = _contextFactory.CreateDbContext();
            context.Invites.Add(new Invite
            {
                SenderID = senderId,
                ReceiverID = receiverId,
                Status = InviteStatus.Pending,
            });
            await context.SaveChangesAsync();
        }
        public async Task SaveChangesToDbAsync()
        {
            using var context = _contextFactory.CreateDbContext();
            _logger.LogInformation("Saving changes to the database.");
            await context.SaveChangesAsync();
        }
        public async Task<List<InviteDTO>> GetInvitesForUserAsync(Guid userId)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Invites
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
            using var context = _contextFactory.CreateDbContext();
            return await context.Invites.FirstOrDefaultAsync(i => i.InviteID == InviteId);
        }
    }
}
