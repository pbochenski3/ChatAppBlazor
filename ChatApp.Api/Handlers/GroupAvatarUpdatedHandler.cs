using ChatApp.Api;
using ChatApp.Application.Interfaces.Chats;
using ChatApp.Application.Notifications;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Handlers
{
    public class GroupAvatarUpdatedHandler : INotificationHandler<GroupAvatarUpdatedNotification>
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IChatService _chatService; 

        public GroupAvatarUpdatedHandler(IHubContext<ChatHub> hubContext, IChatService chatService)
        {
            _hubContext = hubContext;
            _chatService = chatService;
        }

        public async Task Handle(GroupAvatarUpdatedNotification n, CancellationToken ct)
        {
            var usersInChat = await _chatService.GetUsersInChatIdAsync(n.ChatId);
            var usersToNotify = usersInChat.Select(u => u.ToString()).ToList();

            await _hubContext.Clients.Users(usersToNotify)
                .SendAsync("GroupAvatarReload", n.AvatarUrl, n.ChatId, ct);
        }
    }
}
