using ChatApp.Application.Common.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Feature.GroupChat.LeaveGroupChatAsync
{
    public record LeaveGroupChatCommand(Guid ChatId, Guid UserId, string Username) : ICommand<bool>;
}
