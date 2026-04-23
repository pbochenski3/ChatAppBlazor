using ChatApp.Application.DTO;
using ChatApp.Application.Interfaces.Chats;
using ChatApp.Application.Notifications.GroupChat;
using ChatApp.Application.Services.Chats;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Api.Handlers.GroupChat
{
    public class GroupChatCreatedHandler : INotificationHandler<GroupChatCreatedNotification>
    {
        private readonly IHubContext<ChatHub> _hubContext;
        public GroupChatCreatedHandler(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;

        }
        public async Task Handle(GroupChatCreatedNotification n, CancellationToken cancellationToken)
        {
            var usersToNotify = n.usersToNofity.Select(u => u.ToString()).ToList();

            if (n.usersToNofity.Any())
            {
                await _hubContext.Clients.Users(usersToNotify).SendAsync("SidebarChatsReload");
            }

            await _hubContext.Clients.Group(n.chatId.ToString()).SendAsync("SidebarChatsReload");
        }
    }
}
