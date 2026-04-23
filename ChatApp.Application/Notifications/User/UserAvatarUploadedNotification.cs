using MediatR;

namespace ChatApp.Application.Notifications.User
{
    public record UserAvatarUploadedNotification(Guid UserId, string AvatarUrl) : INotification;

}
