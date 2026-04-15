using ChatApp.ChatServer.Client.Services.Api.Interfaces;
using ChatApp.ChatServer.Client.Services.State;

namespace ChatApp.ChatServer.Client.Services.Actions
{
    public class SidebarActionService
    {
        private readonly SidebarStateService _sidebarStateService;
        private readonly IContactApiClient _contactApiClient;
        private readonly ILogger<SidebarActionService> _logger;
        public SidebarActionService(ILogger<SidebarActionService> logger, SidebarStateService sidebarStateService, IContactApiClient contactApiClient)
        {
            _logger = logger;
            _sidebarStateService = sidebarStateService;
            _contactApiClient = contactApiClient;
        }
        public event Action? OnStateChanged;


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
                OnStateChanged?.Invoke();
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
                OnStateChanged?.Invoke();
            }
            else if (!clean)
            {
                await HandleSidebarLoadAsync(true);
            }
        }
    }

}
