using ChatApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Interfaces.Repository
{
    public interface IInviteRepository
    {
        Task SaveChangesToDbAsync();
        Task AddInviteToDB(Guid senderId, Guid receiverId);
    }
}
