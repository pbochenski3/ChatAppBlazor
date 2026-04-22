using ChatApp.Application.Common.Messaging;
using ChatApp.Application.DTO.Requests;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Feature.Chat.UpdateChatName
{
    public record UpdateChatNameCommand(Guid ChatId,Guid UserId, ChangeChatNameRequest Request) : ICommand<bool>;
}
