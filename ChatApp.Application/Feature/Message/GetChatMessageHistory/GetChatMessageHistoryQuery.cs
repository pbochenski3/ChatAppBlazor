using ChatApp.Application.Common.Messaging;
using ChatApp.Application.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Feature.Message.GetChatMessageHistory
{
    public record GetChatMessageHistoryQuery(Guid UserId, Guid ChatId) : IQuery<List<MessageDTO>>;
}
