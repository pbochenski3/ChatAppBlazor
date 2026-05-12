using ChatApp.Application.Interfaces;
using ChatApp.Application.Notifications.Chat;
using MediatR;

namespace ChatApp.Api.NotificationHandlers.Chat
{
    public class UserAdminFlagUpdatedHandler : INotificationHandler<UserAdminFlagUpdatedNotification>
    {
        private readonly ISignalRNotificationService _signalR;
        public UserAdminFlagUpdatedHandler(ISignalRNotificationService signalR) => _signalR = signalR;

        public async Task Handle(UserAdminFlagUpdatedNotification n, CancellationToken cancellationToken)
        {
            await _signalR.SendToGroup(n.ChatId.ToString(), "UpdateFlagOnChat", n.UserId, n.ChatId, n.Flag);

        }
    }
}
