using MediatR;

namespace ChatApp.Application.Notifications.Message
{
    public record MessageDeletedNotification(Guid ChatId, Guid MessageId) : INotification;
}
