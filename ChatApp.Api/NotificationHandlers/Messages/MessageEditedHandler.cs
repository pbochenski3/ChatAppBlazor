using ChatApp.Application.Notifications.Message;
using MediatR;

namespace ChatApp.Api.NotificationHandlers.Messages
{
    public class MessageEditedHandler : INotificationHandler<MessageEditedNotification>
    {
        public Task Handle(MessageEditedNotification notification, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
