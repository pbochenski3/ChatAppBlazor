using ChatApp.Application.Notifications;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Api.Handlers
{
    public class ChatDeletedHandler : INotificationHandler<ChatDeletedNotification>
    {
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatDeletedHandler(IHubContext<ChatHub> hubContext) => _hubContext = hubContext;

        public async Task Handle(ChatDeletedNotification n, CancellationToken ct)
        {
            var user = n.UserId.ToString();

            await Task.WhenAll(
                _hubContext.Clients.User(user).SendAsync("ReceiveStatus", "Czat został usunięty!", ct),
                _hubContext.Clients.User(user).SendAsync("SidebarChatsReload", ct),
                _hubContext.Clients.User(user).SendAsync("ChatClose", ct)
            );
        }
    }
}
