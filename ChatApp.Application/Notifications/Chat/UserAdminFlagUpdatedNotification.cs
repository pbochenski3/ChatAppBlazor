using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Notifications.Chat
{
    public record UserAdminFlagUpdatedNotification(Guid UserId, Guid ChatId, bool Flag) : INotification;
}
