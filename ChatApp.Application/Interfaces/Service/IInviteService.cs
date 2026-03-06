using ChatApp.Application.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Interfaces.Service
{
    public interface IInviteService
    {
        Task SendInvite(Guid senderId, Guid receiverId);
    }
}
