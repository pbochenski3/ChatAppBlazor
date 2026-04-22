using ChatApp.Application.Common.Messaging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace ChatApp.Application.Feature.Chat.MarkAllMessagesAsRead
{
    public record MarkAllMessagesAsReadCommand(Guid UserId, Guid ChatId) : ICommand<bool>;
}
