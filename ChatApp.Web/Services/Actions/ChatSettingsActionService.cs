using ChatApp.Domain.Enums;
using ChatApp.Web.Services.Common;
using ChatApp.Web.Services.Interfaces.Actions;
using ChatApp.Web.Services.Interfaces.Api;
using ChatApp.Web.Services.Interfaces.Common;
using ChatApp.Web.Services.State;

namespace ChatApp.Web.Services.Actions
{
    public class ChatSettingsActionService : IChatSettingsActionService
    {
        private readonly ChatStateService _chatStateService;
        private readonly AppStateService _appStateService;
        private readonly SidebarStateService _sidebarStateService;
        private readonly IChatApiClient _chatApi;
        private readonly IContactApiClient _contactApi;
        private readonly IGroupChatApiClient _groupChatApi;
        private readonly DialogService _dialogService;
        private readonly INotificationService _notification;

        private readonly ILogger<ChatSettingsActionService> _logger;
        public ChatSettingsActionService(
            ChatStateService chatStateService,
            AppStateService appStateService,
            SidebarStateService sidebarStateService,
            IChatApiClient chatApi,
            IGroupChatApiClient groupChatApi,
            DialogService dialogService,
            IContactApiClient contactApi,
            ILogger<ChatSettingsActionService> logger,
            INotificationService notification
            )
        {
            _chatStateService = chatStateService;
            _appStateService = appStateService;
            _sidebarStateService = sidebarStateService;
            _chatApi = chatApi;
            _groupChatApi = groupChatApi;
            _dialogService = dialogService;
            _contactApi = contactApi;
            _logger = logger;
            _notification = notification;
        }
        public event Action? OnStateChanged;
        public void RequestUserRemove()
        {
            _dialogService.ShowConfirm(
                "Usuwanie użytkownika",
                "Czy na pewno chcesz usunąć tego użytkownika z czatu?",
                "Usuń",
                "Anuluj",
                async () => await HandleRemoveUserFromChat()
            );
        }
        public async Task HandleRemoveUserFromChat()
        {
            var userId = _chatStateService.CurrentUserDetails.UserID;
            var userAlias = _chatStateService.CurrentAlias;
            var adminName = _appStateService.CurrentChat.Identity.Alias;
            var chatId = _appStateService.CurrentChat.Identity.ChatID;
            var success = await _groupChatApi.DeleteUserFromChat(chatId, userId, userAlias, adminName);
            if (success)
            {
                _chatStateService.UsersInChat.Remove(_chatStateService.CurrentUserDetails);
                CloseUserDetails();
            }
        }
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
            if (_appStateService.CurrentChat != null)
            {
                await _chatApi.DeleteChatAsync(_appStateService.CurrentChat.Identity.ChatID);
            }
        }
        public void RequestLeaveChat()
        {
            if (_chatStateService.UsersInChat.Count == 1)
            {
                _dialogService.ShowConfirm(
                    "Opuszczanie czatu",
                    "Jesteś ostatnią osobą na czacie. Po Twoim wyjściu grupa zostanie na stałe zablokowana (tryb archiwalny). Nie będzie można dodawać nowych osób ani wysyłać wiadomości. Czy chcesz kontynuować?",
                    "Opuść",
                    "Anuluj",
                    async () => await HandleChatLeaveAsync()
                );
            }
            else
            {
                _dialogService.ShowConfirm(
                    "Opuszczanie czatu",
                    "Czy na pewno chcesz opuścić chat?",
                    "Opuść",
                    "Anuluj",
                    async () => await HandleChatLeaveAsync()
                    );
            }
        }
        public async Task HandleChatLeaveAsync()
        {

            if (_appStateService.CurrentChat != null)
            {
                var chatId = _appStateService.CurrentChat.Identity.ChatID;
                var username = _appStateService.CurrentUser.Username;
                var success = await _groupChatApi.LeaveGroupChatAsync(chatId, username);
                if (success)
                {
                    _appStateService.CurrentChat.State.IsArchive = true;
                    var chat = _sidebarStateService.SidebarItems.FirstOrDefault(s => s.Identity.ChatID == chatId);
                    chat.State.IsArchive = true;
                }
                OnStateChanged?.Invoke();
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
        public async Task HandleContactDeleteAsync(Guid chatId)
        {
            try
            {
                await _contactApi.RemoveContactAsync(chatId);
                _notification.Notify("Pomyślnie usunięto kontakt!", NotificationType.Info);

            }
            catch (Exception ex)
            {
                _notification.Notify("Nie udało się usunąć kontaktu!", NotificationType.Warning);

                _logger.LogError($"[ChatSettingsService] HandleContactDelete {ex}");
            }

        }
        public async Task HandleLoadUsersToAddAsync()
        {
            try
            {
                var chatId = _appStateService.CurrentChat.Identity.ChatID;
                _chatStateService.ReceivedContacts = await _contactApi.GetContactListAsync(chatId);
                _chatStateService.SettingsView = ChatSettingsView.AddUsers;
            }
            catch (Exception ex)
            {
                _notification.Notify("Nie udało się załadować kontaktów!", NotificationType.Warning);

                _logger.LogError($"[ChatSettingsService] HandleLoadUsersToAdd {ex}");

            }

        }
        public async Task HandleChangeChatNameAsync(string chatName)
        {
            var chatId = _appStateService.CurrentChat?.Identity.ChatID;
            var adminName = _appStateService.CurrentUser?.Username;
            var isGroup = _appStateService.CurrentChat.Identity.IsGroup;
            if (chatId != null && adminName != null)
            {
                try
                {
                    await _chatApi.ChangeChatNameAsync(chatId.Value, chatName, adminName, isGroup);
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
        public async Task HandleAddUsersToChatAsync(HashSet<Guid> usersToAdd, AddType type)
        {
            if (_appStateService.CurrentChat != null)
            {
                try
                {
                    if (type == AddType.Group)
                    {
                        await _groupChatApi.AddUsersToGroupChatAsync(_appStateService.CurrentChat.Identity.ChatID, usersToAdd);
                    }
                    else
                    {
                        await _groupChatApi.CreateGroupChatAsync(_appStateService.CurrentChat.Identity.ChatID, usersToAdd);
                    }
                }
                catch (Exception ex)
                {
                    _notification.Notify("Nie udało się dodać użytkownika do czatu!", NotificationType.Warning);

                    _logger.LogError($"[ChatSettingsService] HandleAddUsersToChat {ex}");

                }
            }
        }
        public async Task OpenUserDetails(Guid userId, string alias, string username)
        {
            var chatId = _appStateService.CurrentChat.Identity.ChatID;
            _chatStateService.CurrentAlias = alias;
            _chatStateService.CurrentUsername = username;
            _chatStateService.CurrentUserDetails = _chatStateService.UsersInChat.FirstOrDefault(u => u.UserID == userId);
            _chatStateService.SettingsView = ChatSettingsView.UserDetails;
            OnStateChanged?.Invoke();
        }
        public void CloseUserDetails()
        {
            _chatStateService.CurrentUserDetails = null;
            _chatStateService.CurrentAlias = string.Empty;
            _chatStateService.CurrentUsername = string.Empty;
            _chatStateService.SettingsView = ChatSettingsView.Users;
            OnStateChanged?.Invoke();

        }


    }
}
