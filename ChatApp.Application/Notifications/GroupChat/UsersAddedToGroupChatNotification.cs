using ChatApp.Application.DTO;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Notifications.GroupChat
{
    public record UsersAddedToGroupChatNotification(Guid GroupChatId,Domain.Models.Message SystemMessage, HashSet<Guid> UsersInChat) : INotification;
    
    
}
