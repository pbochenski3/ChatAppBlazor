using ChatApp.Web.Events.Chat;
using ChatApp.Web.Services.State;
using MediatR;

namespace ChatApp.Web.Handlers.Chat
{
    public class ChatMessageDeletedHandler : INotificationHandler<ChatMessegeDeletedNotification>
    {
        private readonly ChatStateService _chatState;
        private readonly AppStateService _appState;
        public ChatMessageDeletedHandler(ChatStateService chatState, AppStateService appState)
        {
            _chatState = chatState;
            _appState = appState;
        }

        public async Task Handle(ChatMessegeDeletedNotification n, CancellationToken cancellationToken)
        {
            if (_appState.CurrentChat?.Identity.ChatID != n.ChatId) return;
            _chatState.DeleteChatMessage(n.MessageId, true);

        }
    }
}
