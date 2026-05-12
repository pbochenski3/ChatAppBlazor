using ChatApp.Application.Interfaces;
using ChatApp.Application.Notifications.Contact;
using MediatR;

namespace ChatApp.Api.NotificationHandlers.Contacts
{

    public class ContactDeletedHandler : INotificationHandler<ContactDeletedNotification>
    {
        private readonly ISignalRNotificationService _signalR;
        public ContactDeletedHandler(ISignalRNotificationService signalR) => _signalR = signalR;

        public async Task Handle(ContactDeletedNotification n, CancellationToken cancellationToken)
        {
            var recipients = new List<string> { n.UserId.ToString(), n.ContactId.ToString() };
            await _signalR.SendToUsers(recipients, "ChatReload", n.ChatId, true);
            await Task.WhenAll(
                _signalR.SendToUser(n.UserId.ToString(), "NotifyInfo", "Kontakt został usunięty!"),
                _signalR.SendToUser(n.ContactId.ToString(), "NotifyInfo", "Ktoś usunął cie z kontaktów!")
            );

            await _signalR.SendToUsers(recipients, "SidebarInvitesReload");
            await _signalR.SendToUsers(recipients, "SidebarChatsReload");
        }
    }
}
