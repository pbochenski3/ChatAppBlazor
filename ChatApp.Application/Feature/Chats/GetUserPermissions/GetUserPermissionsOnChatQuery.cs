using ChatApp.Application.Common.Messaging;

namespace ChatApp.Application.Feature.Chats.GetUserPermissions
{
    public record GetUserPermissionsOnChatQuery(Guid UserId, Guid ChatId) : IQuery<bool>;
}
