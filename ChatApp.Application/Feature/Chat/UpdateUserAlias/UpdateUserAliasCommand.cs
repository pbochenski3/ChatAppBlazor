using ChatApp.Application.DTO.Requests;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Feature.Chat.UpdateUserAlias
{
    public record UpdateUserAliasCommand(Guid ChatId, ChangeAliasRequest request) : BaseCommand<bool>;
}
