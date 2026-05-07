using ChatApp.Web.Events.Chat;
using ChatApp.Web.Services.Interfaces.Actions;
using MediatR;

namespace ChatApp.Web.Handlers.Chat
{
    public class ChatMessageDeletedHandler : INotificationHandler<ChatMessegeDeletedNotification>
    {
        private readonly IChatMessageActionService _messageAction;
        public ChatMessageDeletedHandler(IChatMessageActionService messageAction) => _messageAction = messageAction;

        public async Task Handle(ChatMessegeDeletedNotification n, CancellationToken cancellationToken)
        {
            await _messageAction.HandleMessageDeletedAsync(n.MessageId, n.ChatId);
        }
    }
}
