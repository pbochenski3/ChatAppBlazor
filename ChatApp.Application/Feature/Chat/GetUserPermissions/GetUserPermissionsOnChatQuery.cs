using ChatApp.Application.Common.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Feature.Chat.GetUserPermissions
{
    public record GetUserPermissionsOnChatQuery(Guid UserId, Guid ChatId) : IQuery<bool>;
}
