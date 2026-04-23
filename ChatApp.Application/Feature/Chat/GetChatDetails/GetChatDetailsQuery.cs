using ChatApp.Application.Common.Messaging;
using ChatApp.Application.DTO.Chats;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Feature.Chat.GetChatDetails
{
    public record GetChatDetailsQuery(Guid ChatId, Guid UserId) : IQuery<UserChatDTO>;
}
