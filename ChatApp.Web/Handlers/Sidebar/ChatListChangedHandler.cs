using ChatApp.Web.Events.Sidebar;
using ChatApp.Web.Services.Interfaces.Actions;
using MediatR;

namespace ChatApp.Web.Handlers.Sidebar
{
    public class ChatListChangedHandler : INotificationHandler<ChatListChangedNotification>
    {
        private readonly ISidebarActionService _sidebarActionService;

        public ChatListChangedHandler(ISidebarActionService sidebarActionService)
        {
            _sidebarActionService = sidebarActionService;
        }

        public async Task Handle(ChatListChangedNotification notification, CancellationToken cancellationToken)
        {
            await _sidebarActionService.HandleChatsLoadAsync();
        }
    }
}