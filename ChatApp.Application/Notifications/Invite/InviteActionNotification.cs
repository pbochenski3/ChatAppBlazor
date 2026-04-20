using ChatApp.Application.DTO.Requests;
using ChatApp.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Notifications.Invite
{
    public record InviteActionNotification(Guid SenderId,Guid ReciverId,Guid OldChatId, Guid NewChatId,InviteStatus Response) : INotification;
}
