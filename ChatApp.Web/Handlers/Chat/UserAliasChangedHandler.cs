using ChatApp.Web.Events.Chat;
using ChatApp.Web.Services.Interfaces.Actions;
using MediatR;

namespace ChatApp.Web.Handlers.Chat
{
    public class UserAliasChangedHandler : INotificationHandler<UserAliasChangedNotification>
    {
        private readonly IChatActionService _chatActionService;
        public UserAliasChangedHandler(IChatActionService chatActionService) => _chatActionService = chatActionService;
        public async Task Handle(UserAliasChangedNotification notification, CancellationToken cancellationToken)
        {
            await _chatActionService.HandleUserAliasChangeAsync(notification.ChatId, notification.UserId, notification.NewAlias);

        }
    }
}
