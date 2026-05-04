using MediatR;

namespace ChatApp.Application.Notifications.User
{
    public record UserActionFailedNotification(Guid userId, string message) : INotification;
}
