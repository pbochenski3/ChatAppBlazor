using ChatApp.Application.Interfaces;
using ChatApp.Application.Notifications.GroupChat;
using MediatR;

namespace ChatApp.Api.NotificationHandlers.GroupChat
{
    public class GroupChatCreatedHandler : INotificationHandler<GroupChatCreatedNotification>
    {
        private readonly ISignalRNotificationService _signalR;
        public GroupChatCreatedHandler(ISignalRNotificationService signalR) => _signalR = signalR;
        public async Task Handle(GroupChatCreatedNotification n, CancellationToken cancellationToken)
        {
            var usersToNotify = n.usersToNofity.Select(u => u.ToString()).ToList();

            if (n.usersToNofity.Any())
            {
                await _signalR.SendToUsers(usersToNotify, "SidebarChatsReload");
            }

            await _signalR.SendToGroup(n.chatId.ToString(), "SidebarChatsReload");
        }
    }
}
