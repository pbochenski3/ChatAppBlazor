using ChatApp.Application.Interfaces;
using ChatApp.Application.Notifications.GroupChat;
using MediatR;

namespace ChatApp.Api.NotificationHandlers.GroupChat
{
    public class UserLeavedGroupHandler : INotificationHandler<UserLeavedGroupNotification>
    {
        private readonly ISignalRNotificationService _signalR;
        public UserLeavedGroupHandler(ISignalRNotificationService signalR) => _signalR = signalR;
        public async Task Handle(UserLeavedGroupNotification n, CancellationToken cancellationToken)
        {
            if (n.removed)
            {
                await _signalR.SendToUser(n.UserId.ToString(), "NotifyInfo", "Zostałes wyrzucony z czatu!");
            }
            else
            {
                await _signalR.SendToUser(n.UserId.ToString(), "NotifyInfo", "Opuściłeś czat!");
            }
            await _signalR.SendToUser(n.UserId.ToString(), "ChatReload", n.ChatId, true);
            await _signalR.SendToUser(n.UserId.ToString(), "RequestLeaveGroupSignalR", n.ChatId);
            await Task.WhenAll(
            _signalR.SendToGroup(n.ChatId.ToString(), "ReceiveMessage", n.SystemMessage),
            _signalR.SendToGroup(n.ChatId.ToString(), "UsersInChatReload", n.ChatId)
                );
        }
    }
}
