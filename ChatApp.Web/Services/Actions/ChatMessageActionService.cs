using ChatApp.Web.Services.Interfaces.Actions;
using ChatApp.Web.Services.State;

namespace ChatApp.Web.Services.Actions
{

    public class ChatMessageActionService : IChatMessageActionService
    {
        private readonly ChatStateService _chatState;
        private readonly AppStateService _appState;
        public event Action? OnStateChanged;
        public ChatMessageActionService(ChatStateService chatState, AppStateService appState)
        {
            _chatState = chatState;
            _appState = appState;
        }
        public async Task HandleMessageEditedAsync(Guid messageId, Guid chatId, string content)
        {
            if (_appState.CurrentChat?.Identity.ChatID != chatId) return;
            var message = _chatState.ReceivedMessages.Where(m => m.MessageID == messageId).FirstOrDefault();
            if (message == null) return;
            message.Content = content;
            message.IsEdited = true;
            OnStateChanged?.Invoke();

        }
        public async Task HandleMessageDeletedAsync(Guid messageId, Guid chatId)
        {
            if (_appState.CurrentChat?.Identity.ChatID != chatId) return;
            var message = _chatState.ReceivedMessages.Where(m => m.MessageID == messageId).FirstOrDefault();
            if (message == null) return;
            message.Content = string.Empty;
            message.IsDeleted = true;
            OnStateChanged?.Invoke();
        }
        public async Task RequestMessageDeleted(Guid messageId, Guid chatId)
        {
        }
        public async Task RequestMessageEdited(Guid messageId, Guid chatId, string content)
        {
        }
    }
}
