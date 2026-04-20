using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Notifications.Contact
{
    public record ContactDeletedNotification(Guid ContactId,Guid UserId, Guid ChatId) : INotification;
}
