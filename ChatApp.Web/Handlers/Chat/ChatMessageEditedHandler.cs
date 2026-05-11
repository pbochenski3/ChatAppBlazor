using ChatApp.Web.Events.Chat;
using ChatApp.Web.Services.State;
using MediatR;

namespace ChatApp.Web.Handlers.Chat
{
    public class ChatMessageEditedHandler : INotificationHandler<ChatMessageEditedNotification>
    {
        private readonly ChatStateService _chatState;
        private readonly AppStateService _appState;
        public ChatMessageEditedHandler(ChatStateService chatState, AppStateService appState)
        {
            _chatState = chatState;
            _appState = appState;
        }
        public async Task Handle(ChatMessageEditedNotification n, CancellationToken cancellationToken)
        {
            if (_appState.CurrentChat?.Identity.ChatID != n.ChatId) return;
            _chatState.UpdateChatMessage(n.MessageId, n.Content, true);
        }
    }
}
