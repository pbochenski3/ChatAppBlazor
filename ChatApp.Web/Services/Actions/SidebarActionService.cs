using ChatApp.Application.DTO.Requests;
using ChatApp.Domain.Enums;
using ChatApp.Web.Services.Interfaces.Actions;
using ChatApp.Web.Services.Interfaces.Api;
using ChatApp.Web.Services.Interfaces.Common;
using ChatApp.Web.Services.State;

namespace ChatApp.Web.Services.Actions
{
    public class SidebarActionService : ISidebarActionService
    {
        private readonly SidebarStateService _sidebarStateService;
        private readonly IContactApiClient _contactApiClient;
        private readonly IInviteApiClient _inviteApiClient;
        private readonly AppStateService _appStateService;
        private readonly ILogger<SidebarActionService> _logger;
        private readonly INotificationService _notification;
        public SidebarActionService(
            ILogger<SidebarActionService> logger,
            SidebarStateService sidebarStateService,
            IContactApiClient contactApiClient,
            AppStateService appStateService,
            IInviteApiClient inviteApiClient,
            INotificationService notification

            )
        {
            _logger = logger;
            _appStateService = appStateService;
            _sidebarStateService = sidebarStateService;
            _contactApiClient = contactApiClient;
            _inviteApiClient = inviteApiClient;
            _notification = notification;

        }
        public event Action? OnSidebarStateChanged;
        public event Action? OnInvitesStateChanged;
        public async Task HandleChatsLoadAsync()
        {
            try
            {
                _sidebarStateService.SidebarItems = await _contactApiClient.GetSidebarItemsAsync();

            }
            catch (Exception ex)
            {
                _notification.Notify("Wystąpił bład podczas ładowania listy!", NotificationType.Error);

                _logger.LogError(ex, "Error during sidebar reload");
            }
            finally
            {
                OnSidebarStateChanged?.Invoke();
            }
        }
        public async Task HandleCounterUpdateAsync(Guid chatId, bool clean)
        {
            var items = _sidebarStateService.SidebarItems;
            if (items == null) return;
            var index = items.FindIndex(sb => sb.Identity.ChatID == chatId);

            if (index != -1)
            {
                var item = items[index];
                if (_appStateService.CurrentChat.Identity.ChatID != chatId)
                {
                    item.State.UnreadMessageCount++;
                    items.RemoveAt(index);
                    items.Insert(0, item);
                    _appStateService.SetPageTitle($"Masz {item.State.UnreadMessageCount} nowych wiadomości!");
                }
                if (clean)
                {
                    item.State.UnreadMessageCount = 0;
                    _appStateService.SetPageTitle($"Chat {item.Identity.ChatName}");

                }
                _sidebarStateService.SidebarItems = items;
                OnSidebarStateChanged?.Invoke();
            }
        }
        public async Task HandleInviteResponseAsync(InviteActionRequest request)
        {
            await _inviteApiClient.HandleInviteActionAsync(request);

        }
        public async Task HandleGlobalSearchAsync(string query)
        {
            _sidebarStateService.IsSearchingGlobal = true;
            _sidebarStateService.FoundUsers = await _contactApiClient.GetSearchedUsersList(query);
            _sidebarStateService.IsSearchingGlobal = false;
            OnInvitesStateChanged?.Invoke();
        }
        public async Task HandleInvitesLoadAsync()
        {
            _sidebarStateService.IsPending = true;
            _sidebarStateService.ReceivedInvites = await _inviteApiClient.GetUserInvitesAsync();
            _sidebarStateService.IsPending = false;
            OnSidebarStateChanged?.Invoke();
        }
        public async Task HandleSendInviteAsync(Guid contactId)
        {
            await _inviteApiClient.SendContactInviteAsync(contactId);
            _sidebarStateService.FoundUsers.RemoveAll(u => u.UserID == contactId);
            _sidebarStateService.IsSearchingGlobal = false;
            _sidebarStateService.IsPending = false;
        }
        public async Task HandleSidebarLastMessageReloadAsync(Guid chatId, string sender, string content)
        {
            var items = _sidebarStateService.SidebarItems;
            var itemsToUpdate = items.FirstOrDefault(sb => sb.Identity.ChatID == chatId);
            if (itemsToUpdate != null)
            {
                itemsToUpdate.LastMessage.LastMessageContent = content;
                itemsToUpdate.LastMessage.LastMessageSender = sender;
            }

            OnSidebarStateChanged?.Invoke();

        }
        public async Task HandleChatNameReloadAsync(Guid chatId, string newChatName, Guid userId)
        {
            var chatsToUpdate = _sidebarStateService.SidebarItems
                .Where(c => c.Identity.ChatID == chatId)
                .ToList();

            foreach (var chat in chatsToUpdate)
            {
                if (chat.Identity.IsGroup)
                {
                    chat.Identity.ChatName = newChatName;
                }
                else if (chat.Identity.OtherUserId == userId)
                {
                    chat.Identity.ChatName = newChatName;
                    chat.Identity.OtherUserAlias = newChatName;
                }
            }

            var current = _appStateService.CurrentChat;
            if (current != null && current.Identity.ChatID == chatId)
            {
                if (current.Identity.IsGroup)
                {
                    current.Identity.ChatName = newChatName;
                }
                else if (current.Identity.OtherUserId == userId)
                {
                    current.Identity.ChatName = newChatName;
                    current.Identity.OtherUserAlias = newChatName;
                }
            }

            OnSidebarStateChanged?.Invoke();
        }
        public async Task HandleSidebarLockAsync()
        {
            _sidebarStateService.IsPending = !_sidebarStateService.IsPending;
        }
        public async Task Refresh()
        {
            OnSidebarStateChanged?.Invoke();
        }
    }

}
