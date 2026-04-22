using ChatApp.Application.Common.Messaging;
using ChatApp.Application.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Feature.GroupChat.GetChatUsers
{
    public record GetChatUsersQuery(Guid ChatId) : IQuery<HashSet<UserDTO>>;
}
