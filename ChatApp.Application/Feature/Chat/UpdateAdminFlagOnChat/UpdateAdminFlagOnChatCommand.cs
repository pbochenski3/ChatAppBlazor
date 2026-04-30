using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Feature.Chat.UpdateAdminFlagOnChat
{
    public record UpdateAdminFlagOnChatCommand(Guid ChatId, Guid UserId, bool Flag) : BaseCommand<bool>;
}
