using ChatApp.Application.DTO.Requests;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Notifications.Chat
{
    public record ChatNameUpdatedNotification(Guid ChatId, ChangeChatNameRequest Request) : INotification;
}
