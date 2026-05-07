using ChatApp.Web.Services.Interfaces.Actions;
using ChatApp.Web.Services.Interfaces.Api;
using ChatApp.Web.Services.State;

namespace ChatApp.Web.Services.Actions
{

    public class ChatMessageActionService : IChatMessageActionService
    {
        private readonly ChatStateService _chatState;
        private readonly AppStateService _appState;
        private readonly IMessageApiClient _messageApi;
        public event Action? OnStateChanged;
        public ChatMessageActionService(ChatStateService chatState, AppStateService appState, IMessageApiClient messageApi)
        {
            _chatState = chatState;
            _appState = appState;
            _messageApi = messageApi;
        }
        public async Task HandleMessageEditedAsync(Guid messageId, Guid chatId, string content)
        {
            if (_appState.CurrentChat?.Identity.ChatID != chatId) return;
            var message = _chatState.ReceivedMessages.Where(m => m.MessageID == messageId).FirstOrDefault();
            if (message == null) return;
            message.Content = content;
            message.IsEdited = true;
            Console.WriteLine("edited refresh");
            OnStateChanged?.Invoke();

        }
        public async Task HandleMessageDeletedAsync(Guid messageId, Guid chatId)
        {
            if (_appState.CurrentChat?.Identity.ChatID != chatId) return;
            var message = _chatState.ReceivedMessages.Where(m => m.MessageID == messageId).FirstOrDefault();
            if (message == null) return;
            string content = "Wiadomość została usunięta";
            message.Content = content;
            message.IsDeleted = true;
            Console.WriteLine("deleted refresh");
            OnStateChanged?.Invoke();
        }
        public async Task RequestMessageDeleted(Guid messageId, Guid chatId)
        {
            await _messageApi.DeleteMessageAsync(messageId, chatId);
        }
        public async Task RequestMessageEdited(Guid messageId, Guid chatId, string content)
        {
            await _messageApi.EditMessageAsync(messageId, chatId, content);
        }
    }
}
