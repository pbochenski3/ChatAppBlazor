using ChatApp.Application.DTO;
using ChatApp.Domain.Enums;
using ChatApp.Web.Events.SignalR;
using ChatApp.Web.Services.Interfaces.Actions;
using ChatApp.Web.Services.Interfaces.Api;
using ChatApp.Web.Services.Interfaces.Common;
using ChatApp.Web.Services.State;
using MediatR;

namespace ChatApp.Web.Services.Actions
{
    public class ChatActionService : IChatActionService
    {
        private readonly AppStateService _appStateService;
        private readonly ChatStateService _chatStateService;
        private readonly IChatApiClient _chatApi;
        private readonly IGroupChatApiClient _groupChatApi;
        private readonly ILogger<ChatActionService> _logger;
        private readonly IMediator _mediator;
        private readonly INotificationService _notification;
        private readonly ISidebarActionService _sidebarActionService;
        private readonly SidebarStateService _sidebarStateService;

        public ChatActionService(
            AppStateService appStateService,
            ChatStateService chatStateService,
            ISidebarActionService sidebarActionService,
            IChatApiClient chatApi,
            IGroupChatApiClient groupChatApi,
            ILogger<ChatActionService> logger,
            IMediator mediator,
            INotificationService notification,
            SidebarStateService sidebarStateService

            )
        {
            _appStateService = appStateService;
            _chatStateService = chatStateService;
            _chatApi = chatApi;
            _groupChatApi = groupChatApi;
            _logger = logger;
            _mediator = mediator;
            _notification = notification;
            _sidebarActionService = sidebarActionService;
            _sidebarStateService = sidebarStateService;
        }
        public event Action? OnStateChanged;
        private CancellationTokenSource? _chatLoadingCts;
        public async Task HandleIncomingMessageAsync(MessageDTO dto)
        {
            var _receivedMessages = _chatStateService.ReceivedMessages;
            bool isMessageForCurrentChat = _appStateService.CurrentChat != null && dto.ChatID == _appStateService.CurrentChat.Identity.ChatID;
            if (isMessageForCurrentChat)
            {
                if (_chatStateService.ReceivedMessages.Any(m => m.MessageID == dto.MessageID)) return;

                _chatStateService.AddMessage(dto);
                if (dto.SenderID != _appStateService.CurrentUser?.UserID)
                {
                    MarkAsRead(dto.ChatID, dto.MessageID);
                }
            }
            OnStateChanged?.Invoke();
        }

        public async void MarkAsRead(Guid chatId, Guid messageId)
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
                _appStateService.CurrentChat = await _chatApi.GetChatDetailsAsync(args.ChatId, token);

                if (_appStateService.CurrentChat == null)
                {
                    _notification.Notify("Wystąpił bład podczas ładowania czatu!", NotificationType.Error);
                    return;
                }
                token.ThrowIfCancellationRequested();
                _chatStateService.SetMessageList(await _chatApi.GetChatMessageHistoryAsync(_appStateService.CurrentChat.Identity.ChatID, token));
                token.ThrowIfCancellationRequested();

                if (!_appStateService.CurrentChat.State.IsArchive)
                {
                    try
                    {
                        _chatStateService.UsersInChat = await _groupChatApi.GetChatUsersAsync(_appStateService.CurrentChat.Identity.ChatID);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "[BLAZORHUB] Failed to load users for chat {Id}", args.ChatId);
                    }
                    await _mediator.Publish(new RequestToJoinSignalRNotification(args.ChatId));
                    await _chatApi.MarkAllMessagesAsReadAsync(_appStateService.CurrentChat.Identity.ChatID, token);
                    await _sidebarActionService.HandleCounterUpdateAsync(args.ChatId, true);
                }

            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Chat loading cancelled: {Id}", args.ChatId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Critical error loading chat {Id}", args.ChatId);
            }
            finally
            {
                if (!token.IsCancellationRequested)
                {
                    OnStateChanged?.Invoke();
                }
            }
        }
        public async Task Refresh()
        {
            OnStateChanged?.Invoke();
        }
        public async Task HandleUserOnGroupLoadAsync(Guid chatId)
        {
            if (_appStateService.CurrentChat?.Identity.ChatID != chatId)
            {
                return;
            }
            try
            {
                _chatStateService.UsersInChat = await _groupChatApi.GetChatUsersAsync(chatId);
                OnStateChanged?.Invoke();

            }
            catch (Exception ex)
            {
                _notification.Notify("Wystąpił bład podczas ładowania użytkowników!", NotificationType.Error);

                _logger.LogWarning(ex, "[BLAZORHUB] Failed to load users for chat {Id}", chatId);
            }
        }
        public async Task HandleUserAliasChangeAsync(Guid chatId, Guid userId, string newAlias)
        {
            await _chatStateService.UpdateUserAliasInMessages(userId, newAlias);
            await _sidebarStateService.UpdateSidebarLastMessageAlias(chatId, newAlias, userId);
            OnStateChanged?.Invoke();
        }
        public async Task HandleUpdateFlagOnChatAsync(Guid userId, Guid chatId, bool flag)
        {
            if (_appStateService.CurrentChat?.Identity.ChatID == chatId)
            {
                var selectedUser = _chatStateService.UsersInChat.FirstOrDefault(u => u.UserID == userId);
                if (selectedUser != null)
                {
                    selectedUser.IsAdmin = flag;
                }
                if (_appStateService.CurrentUser?.UserID == userId)
                {
                    _appStateService.CurrentChat.State.IsAdmin = flag;
                }
                OnStateChanged?.Invoke();
            }
        }
        public async Task HandleChatCloseAsync()
        {
            _appStateService.CurrentChat = null;
            OnStateChanged?.Invoke();

        }
    }
}
