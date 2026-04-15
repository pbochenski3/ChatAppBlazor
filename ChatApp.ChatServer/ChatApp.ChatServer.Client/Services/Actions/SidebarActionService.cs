using ChatApp.Application.DTO;
using ChatApp.ChatServer.Client.Services.Api;
using ChatApp.ChatServer.Client.Services.Api.Interfaces;
using ChatApp.ChatServer.Client.Services.State;
using ChatApp.Domain.Enums;

namespace ChatApp.ChatServer.Client.Services.Actions
{
    public class SidebarActionService
    {
        private readonly SidebarStateService _sidebarStateService;
        private readonly IContactApiClient _contactApiClient;
        private readonly IInviteApiClient _inviteApiClient;
        private readonly AppStateService _appStateService;
        private readonly DialogService _dialogService;
        private readonly ILogger<SidebarActionService> _logger;
        public SidebarActionService(
            ILogger<SidebarActionService> logger,
            SidebarStateService sidebarStateService,
            IContactApiClient contactApiClient,
            AppStateService appStateService,
            DialogService dialogService,
            IInviteApiClient inviteApiClient
            )
        {
            _logger = logger;
            _appStateService = appStateService;
            _sidebarStateService = sidebarStateService;
            _contactApiClient = contactApiClient;
            _dialogService = dialogService;
            _inviteApiClient = inviteApiClient;
        }
        public event Action? OnSidebarStateChanged;
        public async Task HandleSidebarLoadAsync(bool reload)
        {
            try
            {        
                    _sidebarStateService.SidebarItems = await _contactApiClient.GetSidebarItemsAsync();
                _logger.LogInformation("Sidebar reloaded. Items count: {Count}", _sidebarStateService.SidebarItems.Count);
   
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during sidebar reload");
            }
            finally
            {
                OnSidebarStateChanged?.Invoke();
            }
        }
        public async Task HandleCounterUpdateAsync(Guid chatId, bool clean)
        {
            var index = _sidebarStateService.SidebarItems.FindIndex(sb => sb.Identity.ChatID == chatId);

            if (index != -1)
            {
                if (clean)
                {
                    _sidebarStateService.SidebarItems[index].State.UnreadMessageCount = 0;
                    _logger.LogInformation("Counter cleared for: {ChatId}", chatId);
                }
                else
                {
                    var updatedItem = _sidebarStateService.SidebarItems[index];
                    updatedItem.State.UnreadMessageCount += 1;

                    _sidebarStateService.SidebarItems.RemoveAt(index);
                    _sidebarStateService.SidebarItems.Insert(0, updatedItem);
                    _logger.LogInformation("Counter increased for: {ChatId}. Current: {Count}", chatId, updatedItem.State.UnreadMessageCount);
                }
                OnSidebarStateChanged?.Invoke();
            }
            else if (!clean)
            {
                await HandleSidebarLoadAsync(true);
            }
        }
        private async Task HandleGlobalSearch(string query)
        {
            _sidebarStateService.IsSearchingGlobal = true;
            _sidebarStateService.FoundUsers = await _httpClient.GetFromJsonAsync<List<UserDTO>>($"api/user/to-invite?query={query}");
            _sidebarStateService.IsSearchingGlobal = false;
            StateHasChanged();
        }
        private async Task HandleInviteLoadAsync(bool reload)
        {
            _sidebarStateService.ReceivedInvites = await _inviteApiClient.GetUserInvitesAsync();
            _sidebarStateService.IsPending = false;
            await InvokeAsync(StateHasChanged);
            _logger.LogInformation("Invite reload triggered for user {Username}", _currentUser.Username);
        }
        private async Task HandleContactInvite(Guid contactId)
        {
            await _inviteApiClient.SendContactInviteAsync(contactId);
            _sidebarStateService.FoundUsers.RemoveAll(u => u.UserID == contactId);
            _sidebarStateService.IsSearchingGlobal = false;
        }
        private async Task HandleSidebarMessageReloadAsync(Guid chatId, string sender, string content)
        {
            var itemsToUpdate = _sidebarStateService.SidebarItems.FirstOrDefault(sb => sb.Identity.ChatID == chatId);
            if (itemsToUpdate != null)
            {
                itemsToUpdate.LastMessage.LastMessageContent = content;
                itemsToUpdate.LastMessage.LastMessageSender = sender;
            }

            OnSidebarStateChanged?.Invoke();

        }
        private async Task HandleSidebarLock()
        {
            _sidebarStateService.IsPending = !_sidebarStateService.IsPending;
        }
    }

}
