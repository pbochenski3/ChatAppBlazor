using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Notifications.User
{
    public record UserAvatarUploadedNotification(Guid UserId,string AvatarUrl) : INotification;
   
}
