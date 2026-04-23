using MediatR;

namespace ChatApp.Application.Notifications.Invite
{
    public record ContactInviteSendedNotification(Guid SenderId, Guid ReceiverId) : INotification;
}
