using ChatApp.Application.Interfaces;
using ChatApp.Application.Notifications.User;
using MediatR;

namespace ChatApp.Api.NotificationHandlers.Users
{
    public class UserActionFailedHandler : INotificationHandler<UserActionFailedNotification>
    {
        private readonly ISignalRNotificationService _signalR;
        public UserActionFailedHandler(ISignalRNotificationService signalR) => _signalR = signalR;

        public async Task Handle(UserActionFailedNotification n, CancellationToken cancellationToken)
        {
            await _signalR.SendToUser(n.userId.ToString(), "NotifyError", n.message);
        }
    }
}
