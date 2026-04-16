using ChatApp.ChatServer.Client.Services.Api;
using ChatApp.ChatServer.Client.Services.Api.Interfaces;
using ChatApp.ChatServer.Client.Services.State;
using ChatApp.Domain.Enums;

namespace ChatApp.ChatServer.Client.Services.Actions
{
    public class ChatSettingsActionService
    {
        private readonly ChatStateService _chatStateService;
        private readonly AppStateService _appStateService;
        private readonly IChatApiClient _chatApi;
        private readonly IContactApiClient _contactApi;
        private readonly IGroupChatApiClient _groupChatApi;
        private readonly DialogService _dialogService;

        private readonly ILogger<ChatSettingsActionService> _logger;
        public ChatSettingsActionService(
            ChatStateService chatStateService,
            AppStateService appStateService,
            IChatApiClient chatApi,
            IGroupChatApiClient groupChatApi,
            DialogService dialogService,
            IContactApiClient contactApi,
            ILogger<ChatSettingsActionService> logger)
        {
            _chatStateService = chatStateService;
            _appStateService = appStateService;
            _chatApi = chatApi;
            _groupChatApi = groupChatApi;
            _dialogService = dialogService;
            _contactApi = contactApi;
            _logger = logger;
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
            try
            {
                if (_appStateService.CurrentChat != null)
                {
                    await _chatApi.DeleteChatAsync(_appStateService.CurrentChat.Identity.ChatID);
                    //await ShowNotificationAsync("Czat został usunięty!");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during chat deletion");
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
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during leaving chat group");
                // _ = ShowNotificationAsync($"{ex.Message}");
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

            await _contactApi.RemoveContactAsync(chatId);

        }

        public async Task HandleLoadUsersToAddAsync()
        {
            _chatStateService.ReceivedContacts =  await _contactApi.GetContactListAsync();
            _chatStateService.SettingsView = ChatSettingsView.AddUsers;
        }
        public async Task HandleChangeChatNameAsync(string chatName)
        {
            var chatId = _appStateService.CurrentChat?.Identity.ChatID;
            var adminName = _appStateService.CurrentUser?.Username;
            if (chatId != null && adminName != null)
            {
                await _chatApi.ChangeChatNameAsync(chatId.Value, chatName, adminName);
            }
        }
        public async Task HandleCreateGroupChatAsync(HashSet<Guid> usersToAdd)
        {
            if (_appStateService.CurrentChat != null)
            {
                await _groupChatApi.AddUsersToGroupChatAsync(_appStateService.CurrentChat.Identity.ChatID, usersToAdd);
            }
        }

    }
}
