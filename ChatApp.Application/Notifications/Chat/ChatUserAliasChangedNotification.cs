using ChatApp.Application.DTO.Requests;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Notifications.Chat
{
    public record ChatUserAliasChangedNotification(Guid ChatId, ChangeAliasRequest Request) : INotification;
}
