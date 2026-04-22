using ChatApp.Application.Common.Messaging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace ChatApp.Application.Feature.GroupChat.AddUsersToGroupChat
{
    public record AddUsersToGroupChatCommand(Guid ChatId, HashSet<Guid> UsersToAdd,Guid UserId) : ICommand<bool>;
}
