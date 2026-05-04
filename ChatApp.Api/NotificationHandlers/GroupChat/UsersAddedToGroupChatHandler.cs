using ChatApp.Application.Notifications.GroupChat;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Api.NotificationHandlers.GroupChat
{
    public class UsersAddedToGroupChatHandler : INotificationHandler<UsersAddedToGroupChatNotification>
    {
        private readonly IHubContext<ChatHub> _hubContext;
        public UsersAddedToGroupChatHandler(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }
        public async Task Handle(UsersAddedToGroupChatNotification n, CancellationToken cancellationToken)
        {
            var newUsersIds = n.UsersInChat.Select(id => id.ToString()).ToList();
            if (newUsersIds.Any())
            {
                await _hubContext.Clients.Users(newUsersIds).SendAsync("SidebarChatsReload");
                await _hubContext.Clients.Users(newUsersIds).SendAsync("ChatReload", n.GroupChatId, true);
            }

            var groupName = n.GroupChatId.ToString();

            await _hubContext.Clients.Group(groupName).SendAsync("UsersInChatReload", n.GroupChatId);

            if (!string.IsNullOrWhiteSpace(n.SystemMessage.Content))
            {
                await _hubContext.Clients.Group(groupName).SendAsync("ReceiveMessage", n.SystemMessage);
            }
        }
    }
}
