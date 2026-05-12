using ChatApp.Application.Interfaces;
using ChatApp.Application.Notifications.Message;
using MediatR;

namespace ChatApp.Api.NotificationHandlers.Messages
{
    public class MessageDeletedHandler : INotificationHandler<MessageDeletedNotification>
    {
        private readonly ISignalRNotificationService _signalR;
        public MessageDeletedHandler(ISignalRNotificationService signalR) => _signalR = signalR;

        public async Task Handle(MessageDeletedNotification n, CancellationToken cancellationToken)
        {
            await _signalR.SendToGroup(n.ChatId.ToString(), "MessageDeleted", n.ChatId, n.MessageId, n.UserId);
        }
    }
}
