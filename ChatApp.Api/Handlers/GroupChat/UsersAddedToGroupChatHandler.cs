using ChatApp.Application.Notifications.GroupChat;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Api.Handlers.GroupChat
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
            var usersToNotify = n.UsersInChat.Select(id => id.ToString()).ToList();

            if (usersToNotify.Any())
            {
                await _hubContext.Clients.Users(usersToNotify).SendAsync("ChatReload", n.GroupChatId, true);
                await _hubContext.Clients.Users(usersToNotify).SendAsync("SidebarChatsReload");
            }

            await _hubContext.Clients.Group(n.GroupChatId.ToString()).SendAsync("ReceiveMessage", n.SystemMessage);
            await _hubContext.Clients.Group(n.GroupChatId.ToString()).SendAsync("UsersInChatReload", n.GroupChatId);
        }
    }
}
