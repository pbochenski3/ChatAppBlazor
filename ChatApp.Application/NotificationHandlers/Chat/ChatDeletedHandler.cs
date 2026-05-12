using ChatApp.Application.Interfaces;
using ChatApp.Application.Notifications.Chat;
using MediatR;

namespace ChatApp.Api.NotificationHandlers.Chat
{
    public class ChatDeletedHandler : INotificationHandler<ChatDeletedNotification>
    {
        private readonly ISignalRNotificationService _signalR;

        public ChatDeletedHandler(ISignalRNotificationService signalR) => _signalR = signalR;

        public async Task Handle(ChatDeletedNotification n, CancellationToken ct)
        {
            var user = n.UserId.ToString();

            await Task.WhenAll(
                _signalR.SendToUser(user, "NotifyInfo", "Czat został usunięty!"),
                _signalR.SendToUser(user, "SidebarChatsReload"),
                _signalR.SendToUser(user, "ChatClose")
            );
        }
    }
}
