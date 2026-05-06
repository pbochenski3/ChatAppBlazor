using ChatApp.Web.Events.Sidebar;
using ChatApp.Web.Services.Interfaces.Actions;
using MediatR;

namespace ChatApp.Web.Handlers.Sidebar
{
    public class ChatNameChangedHandler : INotificationHandler<ChatNameChangedNotification>
    {
        private readonly ISidebarActionService _sidebarActionService;

        public ChatNameChangedHandler(ISidebarActionService sidebarActionService)
        {
            _sidebarActionService = sidebarActionService;
        }

        public async Task Handle(ChatNameChangedNotification notification, CancellationToken cancellationToken)
        {
            await _sidebarActionService.HandleChatNameReloadAsync(notification.ChatId, notification.NewName, notification.UserId);
        }
    }
}