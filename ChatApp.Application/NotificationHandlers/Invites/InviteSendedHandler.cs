using ChatApp.Application.Interfaces;
using ChatApp.Application.Notifications.Invite;
using MediatR;

namespace ChatApp.Api.NotificationHandlers.Invites
{
    public class InviteSendedHandler : INotificationHandler<ContactInviteSendedNotification>
    {
        private readonly ISignalRNotificationService _signalR;
        public InviteSendedHandler(ISignalRNotificationService signalR) => _signalR = signalR;
        public async Task Handle(ContactInviteSendedNotification n, CancellationToken cancellationToken)
        {
            await Task.WhenAll(
               _signalR.SendToUser(n.SenderId.ToString(), "NotifyInfo", "Zaproszenie wysłane!"),
               _signalR.SendToUser(n.ReceiverId.ToString(), "SidebarInvitesReload"),
               _signalR.SendToUser(n.ReceiverId.ToString(), "NotifyInfo", "Otrzymałeś nowe zaproszenie!")
           );
        }
    }
}
