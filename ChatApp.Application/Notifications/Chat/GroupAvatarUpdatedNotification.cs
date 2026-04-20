using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Notifications.Chat
{
    public record GroupAvatarUpdatedNotification(Guid ChatId, string AvatarUrl) : INotification;
}
