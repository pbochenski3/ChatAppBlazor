using ChatApp.Application.Interfaces;
using ChatApp.Application.Notifications.GroupChat;
using MediatR;

namespace ChatApp.Api.NotificationHandlers.GroupChat
{
    public class UsersAddedToGroupChatHandler : INotificationHandler<UsersAddedToGroupChatNotification>
    {
        private readonly ISignalRNotificationService _signalR;
        public UsersAddedToGroupChatHandler(ISignalRNotificationService signalR) => _signalR = signalR;
        public async Task Handle(UsersAddedToGroupChatNotification n, CancellationToken cancellationToken)
        {
            var newUsersIds = n.UsersInChat.Select(id => id.ToString()).ToList();
            if (newUsersIds.Any())
            {
                await _signalR.SendToUsers(newUsersIds, "SidebarChatsReload");
                await _signalR.SendToUsers(newUsersIds, "ChatReload", n.GroupChatId, true);
            }

            var groupName = n.GroupChatId.ToString();

            await _signalR.SendToGroup(groupName, "UsersInChatReload", n.GroupChatId);

            if (!string.IsNullOrWhiteSpace(n.SystemMessage.Content))
            {
                await _signalR.SendToGroup(groupName, "ReceiveMessage", n.SystemMessage);
            }
        }
    }
}
