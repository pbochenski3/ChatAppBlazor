using ChatApp.Application.Notifications.Invite;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Api.NotificationHandlers.Invites
{
    public class InviteSendedHandler : INotificationHandler<ContactInviteSendedNotification>
    {
        private readonly IHubContext<ChatHub> _hubContext;
        public InviteSendedHandler(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }
        public async Task Handle(ContactInviteSendedNotification n, CancellationToken cancellationToken)
        {
            await Task.WhenAll(
               _hubContext.Clients.User(n.SenderId.ToString()).SendAsync("NotifyInfo", "Zaproszenie wysłane!"),
               _hubContext.Clients.User(n.ReceiverId.ToString()).SendAsync("SidebarInvitesReload"),
               _hubContext.Clients.User(n.ReceiverId.ToString()).SendAsync("NotifyInfo", "Otrzymałeś nowe zaproszenie!")
           );
        }
    }
}
