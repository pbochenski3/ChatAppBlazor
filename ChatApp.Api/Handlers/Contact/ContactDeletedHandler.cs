using ChatApp.Application.Notifications.Contact;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Api.Handlers.Contact
{

    public class ContactDeletedHandler : INotificationHandler<ContactDeletedNotification>
    {
        private readonly IHubContext<ChatHub> _hubContext;
        public ContactDeletedHandler(IHubContext<ChatHub> hubContext) => _hubContext = hubContext;
        public async Task Handle(ContactDeletedNotification n, CancellationToken cancellationToken)
        {
            var recipients = new List<string> { n.UserId.ToString(), n.ContactId.ToString() };
            await _hubContext.Clients.Users(recipients).SendAsync("ChatReload", n.ChatId, true);
            await Task.WhenAll(
                _hubContext.Clients.Users(n.UserId.ToString()).SendAsync("ReceiveStatus", "Kontakt został usunięty!"),
                _hubContext.Clients.Users(n.ContactId.ToString()).SendAsync("ReceiveStatus", "Ktoś usunął cie z kontaktów!")
            );

            await _hubContext.Clients.Users(recipients).SendAsync("SidebarInvitesReload");
            await _hubContext.Clients.Users(recipients).SendAsync("SidebarChatsReload");
        }
    }
}
