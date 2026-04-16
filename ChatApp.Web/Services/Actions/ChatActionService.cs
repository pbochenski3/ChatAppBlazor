using ChatApp.Application.DTO;
using ChatApp.Application.Events;
using ChatApp.Web.Services.Api.Interfaces;
using ChatApp.Web.Services.State;

namespace ChatApp.Web.Services.Actions
{
    public class ChatActionService
    {
        private readonly AppStateService _appStateService;
        private readonly ChatStateService _chatStateService;
        private readonly SidebarActionService _sidebarActionService;
        private readonly IChatApiClient _chatApi;
        private readonly IGroupChatApiClient _groupChatApi;
        private readonly ILogger<ChatActionService> _logger;
        public ChatActionService(
            AppStateService appStateService,
            ChatStateService chatStateService,
            IChatApiClient chatApi,
            IGroupChatApiClient groupChatApi,
            ILogger<ChatActionService> logger,
            SidebarActionService sidebarActionService
            )
        {
            _appStateService = appStateService;
            _chatStateService = chatStateService;
            _chatApi = chatApi;
            _groupChatApi = groupChatApi;
            _logger = logger;
            _sidebarActionService = sidebarActionService;
        }
        public event Func<Guid, Task>? OnJoinGroupRequested;
        public Action? OnStateChanged;
        private CancellationTokenSource? _chatLoadingCts;
        public async Task HandleIncomingMessageAsync(MessageDTO dto)
        {
            var _receivedMessages = _chatStateService.ReceivedMessages;
            bool isMessageForCurrentChat = _appStateService.CurrentChat != null && dto.ChatID == _appStateService.CurrentChat.Identity.ChatID;
            if (isMessageForCurrentChat)
            {
                if (_chatStateService.ReceivedMessages.Any(m => m.MessageID == dto.MessageID)) return;

                _chatStateService.AddMessage(dto);
                if (dto.SenderID != _appStateService.CurrentUser.UserID)
                {
                    MarkAsRead(dto.ChatID, dto.MessageID);
                }
            }
            else if (dto.SenderID != _appStateService.CurrentUser.UserID)
            {
                await _sidebarActionService.HandleCounterUpdateAsync(dto.ChatID, false);
            }
        }

        private async void MarkAsRead(Guid chatId, Guid messageId)
        {
            try
            {
                await _chatApi.MarkMessageAsReadAsync(chatId, messageId);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to mark message {Id} as read", messageId);
            }
        }
        public async Task HandleChatLoadAsync(ContactSelectedArgs args)
        {

            bool isSameChat = _appStateService.CurrentChat?.Identity.ChatID == args.ChatId;

            if (!args.Force && isSameChat)
            {
                return;
            }
            if (args.Force && !isSameChat)
            {
                return;
            }
            if (_appStateService.IsProfileOpen)
            {
                _appStateService.IsProfileOpen = false;
            }

            _chatStateService.IsSettingsOpen = false;
            _chatLoadingCts?.Cancel();
            _chatLoadingCts = new CancellationTokenSource();
            var token = _chatLoadingCts.Token;

            try
            {
                _chatStateService.SetMessageList(new List<MessageDTO>());
                _logger.LogInformation("Loading chat: {Id} (Force: {Force})", args.ChatId, args.Force);

                var chatInformation = await _chatApi.GetChatDetailsAsync(args.ChatId, token);

                if (chatInformation == null)
                {
                    _logger.LogError("[BLAZORHUB] Chat not found: {Id}", args.ChatId);
                    return;
                }

                token.ThrowIfCancellationRequested();

                try
                {
                    _chatStateService.UsersInChat = await _groupChatApi.GetChatUsersAsync(chatInformation.Identity.ChatID);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "[BLAZORHUB] Failed to load users for chat {Id}", args.ChatId);
                }

                _chatStateService.SetMessageList(await _chatApi.GetChatMessageHistoryAsync(chatInformation.Identity.ChatID, token));
                token.ThrowIfCancellationRequested();

                _appStateService.CurrentChat = chatInformation;

                await _appStateService.SetChatAsync(chatInformation);
                if (!chatInformation.State.IsArchive)
                {
                    _chatStateService.IsChatLocked = chatInformation.State.IsArchive;
                    if (OnJoinGroupRequested != null)
                    {
                        await OnJoinGroupRequested.Invoke(chatInformation.Identity.ChatID);
                    }
                    await _chatApi.MarkAllMessagesAsReadAsync(chatInformation.Identity.ChatID, token);
                    await _sidebarActionService.HandleCounterUpdateAsync(chatInformation.Identity.ChatID, true);
                }

            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Chat loading cancelled: {Id}", args.ChatId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Critical error loading chat {Id}", args.ChatId);
               // _ = ShowNotificationAsync("Błąd podczas ładowania czatu.");
            }
            finally
            {
                if (!token.IsCancellationRequested)
                {
                    _logger.LogDebug("Invoking OnStateChanged in ChatActionService for chat {ChatId}. InstanceHash={Hash}", args.ChatId, this.GetHashCode());
                    OnStateChanged?.Invoke();
                }
            }
        }

        public async Task HandleChatClose(bool close)
        {
            await _appStateService.SetChatAsync(null);
        }

    }
}
