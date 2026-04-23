using MediatR;

namespace ChatApp.Application.Notifications.Contact
{
    public record ContactDeletedNotification(Guid ContactId, Guid UserId, Guid ChatId) : INotification;
}
