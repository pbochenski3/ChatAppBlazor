using MediatR;

namespace ChatApp.Application.Notifications.Message
{
    public record MessageEditedNotification(Guid ChatId, Guid MessageId, string Content, DateTime editedAt) : INotification;
}
