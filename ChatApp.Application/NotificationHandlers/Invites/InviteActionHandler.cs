using ChatApp.Application.Interfaces;
using ChatApp.Application.Notifications.Invite;
using ChatApp.Domain.Enums;
using MediatR;

namespace ChatApp.Api.NotificationHandlers.Invites
{
    public class InviteActionHandler : INotificationHandler<InviteActionNotification>
    {
        private readonly ISignalRNotificationService _signalR;
        public InviteActionHandler(ISignalRNotificationService signalR) => _signalR = signalR;
        public async Task Handle(InviteActionNotification n, CancellationToken cancellationToken)
        {
            await Task.WhenAll(
                _signalR.SendToUser(n.SenderId.ToString(), "NotifyInfo", $"Twoje zaproszenie zostało {(n.Response == InviteStatus.Accepted ? "zaakceptowane" : "odrzucone")}"),
                _signalR.SendToUser(n.ReciverId.ToString(), "NotifyInfo", $"{(n.Response == InviteStatus.Accepted ? "Zaakceptowałeś" : "Odrzuciłeś")} zaproszenie!")
            );
            await Task.WhenAll(
                _signalR.SendToUser(n.SenderId.ToString(), "SidebarInvitesReload"),
                _signalR.SendToUser(n.ReciverId.ToString(), "SidebarInvitesReload"),
                _signalR.SendToUser(n.ReciverId.ToString(), "SidebarChatsReload"),
                _signalR.SendToUser(n.SenderId.ToString(), "SidebarChatsReload")
            );

            await Task.WhenAll(
                _signalR.SendToUser(n.ReciverId.ToString(), "ChatReload", n.NewChatId, true),
                _signalR.SendToUser(n.SenderId.ToString(), "ChatReload", n.NewChatId, true)
            );
        }
    }
}
