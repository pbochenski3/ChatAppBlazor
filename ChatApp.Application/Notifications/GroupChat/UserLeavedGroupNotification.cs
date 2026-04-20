using ChatApp.Application.DTO;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Notifications.GroupChat
{
    public record UserLeavedGroupNotification(Guid ChatId,MessageDTO SystemMessage,Guid UserId) : INotification;
}
