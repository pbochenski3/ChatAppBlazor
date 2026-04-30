using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Feature.GroupChat.RemoveUserFromGroup
{
    public record RemoveUserFromGroupCommand(Guid ChatId, Guid UserId,Guid AdminId, string RemovedUserAlias, string AdminName) : BaseCommand<bool>;
}
