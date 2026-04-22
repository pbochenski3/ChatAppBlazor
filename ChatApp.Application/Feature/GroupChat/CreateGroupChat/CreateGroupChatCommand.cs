using ChatApp.Application.Common.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Feature.GroupChat.CreateGroupChat
{
    public record CreateGroupChatCommand(Guid ChatId, HashSet<Guid> UsersToAdd) : ICommand<bool>;
}
