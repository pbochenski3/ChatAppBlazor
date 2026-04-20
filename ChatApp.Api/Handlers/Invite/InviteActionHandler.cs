using Azure.Core;
using ChatApp.Application.Notifications.Invite;
using ChatApp.Domain.Enums;
using ChatApp.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Api.Handlers.Invite
{
    public class InviteActionHandler : INotificationHandler<InviteActionNotification>
    {
        private readonly IHubContext<ChatHub> _hubContext;
        public InviteActionHandler(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }
        public async Task Handle(InviteActionNotification n, CancellationToken cancellationToken)
        {
            await Task.WhenAll(
                _hubContext.Clients.User(n.SenderId.ToString()).SendAsync("ReceiveStatus", $"Twoje zaproszenie zostało {(n.Response == InviteStatus.Accepted ? "zaakceptowane" : "odrzucone")}"),
                _hubContext.Clients.User(n.ReciverId.ToString()).SendAsync("ReceiveStatus", $"{(n.Response == InviteStatus.Accepted ? "Zaakceptowałeś" : "Odrzuciłeś")} zaproszenie!")
            );
            await Task.WhenAll(
                _hubContext.Clients.User(n.SenderId.ToString()).SendAsync("SidebarInvitesReload"),
                _hubContext.Clients.User(n.ReciverId.ToString()).SendAsync("SidebarInvitesReload"),
                _hubContext.Clients.User(n.ReciverId.ToString()).SendAsync("SidebarChatsReload"),
                _hubContext.Clients.User(n.SenderId.ToString()).SendAsync("SidebarChatsReload")
            );

            if (n.NewChatId == n.OldChatId)
            {
                await Task.WhenAll(
                    _hubContext.Clients.User(n.ReciverId.ToString()).SendAsync("ChatReload", n.NewChatId, true),
                    _hubContext.Clients.User(n.SenderId.ToString()).SendAsync("ChatReload", n.NewChatId, true)
                );
            }
        }
    }
}
