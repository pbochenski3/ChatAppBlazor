using ChatApp.Application.Notifications.Message;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Api.NotificationHandlers.Messages
{
    public class MessageDeletedHandler : INotificationHandler<MessageDeletedNotification>
    {
        private readonly IHubContext<ChatHub> _hubContext;
        public MessageDeletedHandler(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Handle(MessageDeletedNotification n, CancellationToken cancellationToken)
        {
            await _hubContext.Clients.Group(n.ChatId.ToString()).SendAsync("MessageDeleted", n.ChatId, n.MessageId, n.UserId);
        }
    }
}
