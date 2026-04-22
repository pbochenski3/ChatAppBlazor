using ChatApp.Application.Common.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Feature.Chat.GetChatUsers
{
    public record GetChatUsersQuery(Guid ChatId) : IQuery<HashSet<Guid>>;
}
