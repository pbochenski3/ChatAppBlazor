using ChatApp.Application.Notifications.Invite;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Api.Handlers.Invite
{
    public class ContactInviteSendedHandler : INotificationHandler<ContactInviteSendedNotification>
    {
        private readonly IHubContext<ChatHub> _hubContext;
        public ContactInviteSendedHandler(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }
        public async Task Handle(ContactInviteSendedNotification n, CancellationToken cancellationToken)
        {
            await Task.WhenAll(
               _hubContext.Clients.User(n.SenderId.ToString()).SendAsync("ReceiveStatus", "Zaproszenie wysłane!"),
               _hubContext.Clients.User(n.ReceiverId.ToString()).SendAsync("SidebarInvitesReload"),
               _hubContext.Clients.User(n.ReceiverId.ToString()).SendAsync("ReceiveStatus", "Otrzymałeś nowe zaproszenie!")
           );
        }
    }
}
