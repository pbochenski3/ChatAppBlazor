using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Notifications
{
    public record GroupAvatarUpdatedNotification(Guid ChatId, string AvatarUrl) : INotification;
}
