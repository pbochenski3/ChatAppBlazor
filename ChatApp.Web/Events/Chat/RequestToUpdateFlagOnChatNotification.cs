using MediatR;

namespace ChatApp.Web.Events.Chat
{
    public record RequestToUpdateFlagOnChatNotification(Guid UserId, Guid ChatId, bool Flag) : INotification;
}
