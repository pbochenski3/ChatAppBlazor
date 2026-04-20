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
        private readonly IGroupChatService _groupChatService;
        public GroupChatCreatedHandler(IHubContext<ChatHub> hubContext,IGroupChatService groupChatService)
        {
            _hubContext = hubContext;
            _groupChatService = groupChatService;

        }
        public async Task Handle(GroupChatCreatedNotification n, CancellationToken cancellationToken)
        {
            var usersInNewChat = await _groupChatService.ProccesGetChatUsersAsync(n.chatId);
            var usersToNotify = usersInNewChat.Select(u => u.UserID.ToString()).ToList();

            if (usersToNotify.Any())
            {
                await _hubContext.Clients.Users(usersToNotify).SendAsync("SidebarChatsReload");
            }

            await _hubContext.Clients.Group(n.chatId.ToString()).SendAsync("SidebarChatsReload");
        }
    }
}
