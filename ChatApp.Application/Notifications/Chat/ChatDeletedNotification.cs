using MediatR;

namespace ChatApp.Application.Notifications.Chat
{
    public record ChatDeletedNotification(Guid ChatId, Guid UserId) : INotification;
}
