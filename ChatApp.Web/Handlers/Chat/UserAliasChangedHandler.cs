using ChatApp.Web.Events.Chat;
using ChatApp.Web.Services.Interfaces.Actions;
using ChatApp.Web.Services.State;
using MediatR;

namespace ChatApp.Web.Handlers.Chat
{
    public class UserAliasChangedHandler : INotificationHandler<UserAliasChangedNotification>
    {
        private readonly IChatActionService _chatActionService;
        private readonly SidebarStateService _sidebarState;
        public UserAliasChangedHandler(IChatActionService chatActionService, SidebarStateService sidebarState)
        {
            _chatActionService = chatActionService;
            _sidebarState = sidebarState;
        }
        public async Task Handle(UserAliasChangedNotification notification, CancellationToken cancellationToken)
        {
            await _chatActionService.HandleUserAliasChangeAsync(notification.ChatId, notification.UserId, notification.NewAlias);
        }
    }
}
