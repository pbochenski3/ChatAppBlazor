using ChatApp.Domain.Enums;
using MediatR;

namespace ChatApp.Application.Notifications.Invite
{
    public record InviteActionNotification(Guid SenderId, Guid ReciverId, Guid OldChatId, Guid NewChatId, InviteStatus Response) : INotification;
}
