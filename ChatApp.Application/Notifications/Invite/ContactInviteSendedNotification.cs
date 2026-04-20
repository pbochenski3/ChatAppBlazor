using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Notifications.Invite
{
    public record ContactInviteSendedNotification(Guid SenderId, Guid ReceiverId) : INotification;
}
