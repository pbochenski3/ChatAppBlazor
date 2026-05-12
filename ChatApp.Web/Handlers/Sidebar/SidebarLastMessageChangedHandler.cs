using ChatApp.Web.Events.Sidebar;
using ChatApp.Web.Services.Interfaces.Actions;
using MediatR;

namespace ChatApp.Web.Handlers.Sidebar
{
    public class SidebarLastMessageChangedHandler : INotificationHandler<SidebarLastMessageChangedNotification>
    {
        private readonly ISidebarActionService _sidebarActionService;

        public SidebarLastMessageChangedHandler(ISidebarActionService sidebarActionService)
        {
            _sidebarActionService = sidebarActionService;
        }

        public async Task Handle(SidebarLastMessageChangedNotification notification, CancellationToken cancellationToken)
        {
            await _sidebarActionService.HandleSidebarLastMessageReloadAsync(notification.Message.ChatID, notification.Message.Alias, notification.Message.Content, notification.Message.SenderID);
            await _sidebarActionService.HandleCounterUpdateAsync(notification.Message.ChatID, false);
        }
    }
}