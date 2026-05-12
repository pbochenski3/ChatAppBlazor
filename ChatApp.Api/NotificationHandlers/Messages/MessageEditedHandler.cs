using ChatApp.Application.Notifications.Message;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Api.NotificationHandlers.Messages
{
    public class MessageEditedHandler : INotificationHandler<MessageEditedNotification>
    {
        private readonly IHubContext<ChatHub> _hubContext;
        public MessageEditedHandler(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Handle(MessageEditedNotification n, CancellationToken cancellationToken)
        {
            await _hubContext.Clients.Group(n.ChatId.ToString()).SendAsync("MessageEdited", n.ChatId, n.MessageId, n.Content, n.UserId);
        }
    }
}
