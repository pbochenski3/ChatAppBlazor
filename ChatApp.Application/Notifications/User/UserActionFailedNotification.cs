using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Notifications.User
{
    public record UserActionFailedNotification(Guid userId, string message) : INotification;
}
