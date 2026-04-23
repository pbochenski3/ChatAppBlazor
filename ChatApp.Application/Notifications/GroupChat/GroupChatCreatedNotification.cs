using ChatApp.Application.DTO;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Notifications.GroupChat
{
    public record GroupChatCreatedNotification(Guid chatId,HashSet<Guid> usersToNofity) : INotification;
}
