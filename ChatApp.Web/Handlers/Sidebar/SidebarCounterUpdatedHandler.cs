using ChatApp.Web.Events.Sidebar;
using ChatApp.Web.Services.Interfaces.Actions;
using MediatR;

namespace ChatApp.Web.Handlers.Sidebar
{
    public class SidebarCounterUpdatedHandler : INotificationHandler<SidebarCounterUpdatedNotification>
    {
        private readonly ISidebarActionService _sidebarActionService;

        public SidebarCounterUpdatedHandler(ISidebarActionService sidebarActionService)
        {
            _sidebarActionService = sidebarActionService;
        }

        public async Task Handle(SidebarCounterUpdatedNotification notification, CancellationToken cancellationToken)
        {
            await _sidebarActionService.HandleCounterUpdateAsync(notification.ChatId, notification.Status);
        }
    }
}