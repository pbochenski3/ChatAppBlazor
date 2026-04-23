using MediatR;

namespace ChatApp.Application.Notifications.Chat
{
    public record GroupAvatarUpdatedNotification(Guid ChatId, string AvatarUrl) : INotification;
}
