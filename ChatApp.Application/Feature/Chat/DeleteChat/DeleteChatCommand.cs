using ChatApp.Application.Common.Messaging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace ChatApp.Application.Feature.Chat.DeleteChat
{
    public record DeleteChatCommand(Guid ChatId, Guid UserId) : BaseCommand<bool>;
}
