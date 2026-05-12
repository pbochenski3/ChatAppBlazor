using ChatApp.Application.Interfaces;
using ChatApp.Application.Notifications.Message;
using MediatR;

namespace ChatApp.Api.NotificationHandlers.Messages
{
    public class MessageEditedHandler : INotificationHandler<MessageEditedNotification>
    {
        private readonly ISignalRNotificationService _signalR;
        public MessageEditedHandler(ISignalRNotificationService signalR) => _signalR = signalR;

        public async Task Handle(MessageEditedNotification n, CancellationToken cancellationToken)
        {
            await _signalR.SendToGroup(n.ChatId.ToString(), "MessageEdited", n.ChatId, n.MessageId, n.Content, n.UserId);
        }
    }
}
