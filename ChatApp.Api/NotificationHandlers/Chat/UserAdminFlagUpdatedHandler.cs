using ChatApp.Application.Notifications.Chat;
using ChatApp.Domain.Interfaces.Repository;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Api.NotificationHandlers.Chat
{
    public class UserAdminFlagUpdatedHandler : INotificationHandler<UserAdminFlagUpdatedNotification>
    {
        private readonly IHubContext<ChatHub> _hubContext;

        public UserAdminFlagUpdatedHandler(IHubContext<ChatHub> hubContext) => _hubContext = hubContext;
        public async Task Handle(UserAdminFlagUpdatedNotification n, CancellationToken cancellationToken)
        {
            await _hubContext.Clients.Group(n.ChatId.ToString()).SendAsync("UpdateFlagOnChat",n.UserId,n.ChatId,n.Flag);

        }
    }
}
