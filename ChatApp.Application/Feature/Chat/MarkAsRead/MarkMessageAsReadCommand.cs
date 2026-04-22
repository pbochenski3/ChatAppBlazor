using ChatApp.Application.Common.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Feature.Chat.MarkAsRead
{
    public record MarkMessageAsReadCommand(Guid UserId, Guid ChatId, Guid MessageId) : ICommand<bool>;
}
