
using ChatApp.Domain.Enums;
using ChatApp.Web.Services.Api.Interfaces;
using ChatApp.Web.Services.Common;
using ChatApp.Web.Services.Common.Interfaces;
using ChatApp.Web.Services.State;
using MediatR;
using static ChatApp.Web.Events.ChatEvents;

namespace ChatApp.Web.Services.Actions
{
    public class ChatSettingsActionService
    {
        private readonly ChatStateService _chatStateService;
        private readonly AppStateService _appStateService;
        private readonly IChatApiClient _chatApi;
        private readonly IContactApiClient _contactApi;
        private readonly IGroupChatApiClient _groupChatApi;
        private readonly DialogService _dialogService;
        private readonly IMediator _mediator;
        private readonly INotificationService _notification;

        private readonly ILogger<ChatSettingsActionService> _logger;
        public ChatSettingsActionService(
            ChatStateService chatStateService,
            AppStateService appStateService,
            IChatApiClient chatApi,
            IGroupChatApiClient groupChatApi,
            DialogService dialogService,
            IContactApiClient contactApi,
            IMediator mediator,
            ILogger<ChatSettingsActionService> logger,
            INotificationService notification
            )
        {
            _chatStateService = chatStateService;
            _appStateService = appStateService;
            _chatApi = chatApi;
            _groupChatApi = groupChatApi;
            _dialogService = dialogService;
            _contactApi = contactApi;
            _logger = logger;
            _mediator = mediator;
            _notification = notification;
        }
        public event Action? OnStateChanged;
        public void RequestDeleteChat()
        {
            _dialogService.ShowConfirm(
                "Usuwanie czatu",
                "Czy na pewno chcesz usunąć ten czat? Ta operacja jest nieodwracalna.",
                "Usuń",
                "Anuluj",
                async () => await HandleChatDeleteAsync()
            );
        }
        public async Task HandleChatDeleteAsync()
        {
            try
            {
                if (_appStateService.CurrentChat != null)
                {
                    await _chatApi.DeleteChatAsync(_appStateService.CurrentChat.Identity.ChatID);
                    _notification.Notify("Czat został pomyślnie usunięty!", NotificationType.Info);
                }
            }
            catch (Exception ex)
            {
                _notification.Notify("Nie udało się usunąć czatu!!", NotificationType.Error);

                _logger.LogError($"[ChatSettingsService] HandleChatDelete {ex}");

            }
        }
        public void RequestLeaveChat()
        {
            _dialogService.ShowConfirm(
                "Opuszczanie czatu",
                "Czy na pewno chcesz opuścić chat?",
                "Opuść",
                "Anuluj",
                async () => await HandleChatLeaveAsync()
            );
        }
        public async Task HandleChatLeaveAsync()
        {
            try
            {
                if (_appStateService.CurrentChat != null)
                {
                    var chatId = _appStateService.CurrentChat.Identity.ChatID;
                    var username = _appStateService.CurrentUser.Username;
                    await _groupChatApi.LeaveGroupChatAsync(chatId, username);
                    _appStateService.CurrentChat.State.IsArchive = true;
                    _notification.Notify("Pomyślnie opuszczono czat!", NotificationType.Info);

                    OnStateChanged?.Invoke();
                }
                    
            }
            catch (Exception ex)
            {
                _notification.Notify("Nie udało się opuścić czatu!", NotificationType.Warning);

                _logger.LogError($"[ChatSettingsService] HandleChatLeave {ex}");


            }
        }
        public void RequestContactDelete(Guid chatId)
        {
            _dialogService.ShowConfirm(
        "Usuwanie kontaktu",
        "Czy na pewno chcesz usunąć ten kontakt?",
        "Usuń",
        "Anuluj",
        async () => await HandleContactDeleteAsync(chatId)
        );
        }
        private async Task HandleContactDeleteAsync(Guid chatId)
        {

                _logger.LogError($"[ChatSettingsService] HandleContactDelete {ex}");
            }

        }

        public async Task HandleLoadUsersToAddAsync()
        {
            try
            {
                _chatStateService.ReceivedContacts = await _contactApi.GetContactListAsync();
                _chatStateService.SettingsView = ChatSettingsView.AddUsers;
            }
            catch(Exception ex)
            {
                _notification.Notify("Nie udało się załadować kontaktów!", NotificationType.Warning);

                _logger.LogError($"[ChatSettingsService] HandleLoadUsersToAdd {ex}");

            }

        }
        public async Task HandleChangeChatNameAsync(string chatName)
        {
            var chatId = _appStateService.CurrentChat?.Identity.ChatID;
            var adminName = _appStateService.CurrentUser?.Username;
            if (chatId != null && adminName != null)
            {
                try
                {
                await _chatApi.ChangeChatNameAsync(chatId.Value, chatName, adminName);
                    _notification.Notify("Nazwa czatu zmieniona pomyślnie!", NotificationType.Info);


                }
                catch (Exception ex)
                {
                    _notification.Notify("Nie udało się zmienić nazwy czatu!", NotificationType.Warning);

                    _logger.LogError($"[ChatSettingsService] HandleChangeChatNameAsync {ex}");
                }
            }
            OnStateChanged?.Invoke();
        }
        public async Task HandleAddUsersToChatAsync(HashSet<Guid> usersToAdd)
        {
            if (_appStateService.CurrentChat != null)
            {
                try
                {
                await _groupChatApi.AddUsersToGroupChatAsync(_appStateService.CurrentChat.Identity.ChatID, usersToAdd);
                }
                catch(Exception ex)
                {
                    _notification.Notify("Nie udało się dodać użytkownika do czatu!", NotificationType.Warning);

                    _logger.LogError($"[ChatSettingsService] HandleAddUsersToChat {ex}");

                }
            }
        }

  
    }
}
