using MediatR;

namespace ChatApp.Web.Events.Chat
{
    public record ChatMessageEditedNotification(Guid MessageId, Guid ChatId, string Content) : INotification;
}
