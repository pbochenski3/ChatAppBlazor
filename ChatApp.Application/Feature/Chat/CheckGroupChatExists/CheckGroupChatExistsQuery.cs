using ChatApp.Application.Common.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Feature.Chat.CheckGroupChatExists
{
    public record CheckGroupChatExistsQuery(Guid ChatId, Guid UserId) : IQuery<bool>;
}
