using ChatApp.Application.DTO.Requests;

namespace ChatApp.Application.Feature.Chats.UpdateUserAlias
{
    public record UpdateUserAliasCommand(Guid ChatId, ChangeAliasRequest request) : BaseCommand<bool>;
}
