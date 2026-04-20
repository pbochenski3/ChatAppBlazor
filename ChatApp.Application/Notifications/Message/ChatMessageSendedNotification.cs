using ChatApp.Application.DTO;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Notifications.Message
{
    public record ChatMessageSendedNotification(MessageDTO Message) : INotification;
}
