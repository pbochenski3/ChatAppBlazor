using MediatR;

namespace ChatApp.Web.Events.Chat
{
    public record UserAliasChangedNotification(Guid ChatId, Guid UserId, string NewAlias) : INotification;

}
