using ChatApp.Application.Notifications.User;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Api.Handlers.User
{
    public class UserActionFailedHandler : INotificationHandler<UserActionFailedNotification>
    {
        private readonly IHubContext<ChatHub> _hubContext;
        public UserActionFailedHandler(IHubContext<ChatHub> hubContext) => _hubContext = hubContext;
             
        public async Task Handle(UserActionFailedNotification n, CancellationToken cancellationToken)
        {
            await _hubContext.Clients.User(n.userId.ToString()).SendAsync("ReceiveStatus", n.message);

        }
    }
}
