using ChatApp.Web.Events.Chat;
using ChatApp.Web.Services.Interfaces.Actions;
using MediatR;

namespace ChatApp.Web.Handlers.Chat
{
    public class ChatMessageEditedHandler : INotificationHandler<ChatMessageEditedNotification>
    {
        private readonly IChatMessageActionService _messageAction;
        public ChatMessageEditedHandler(IChatMessageActionService messageAction) => _messageAction = messageAction;
        public async Task Handle(ChatMessageEditedNotification n, CancellationToken cancellationToken)
        {
            await _messageAction.HandleMessageEditedAsync(n.MessageId, n.ChatId, n.Content);
        }
    }
}
