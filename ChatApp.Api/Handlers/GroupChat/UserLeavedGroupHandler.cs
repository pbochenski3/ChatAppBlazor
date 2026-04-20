using ChatApp.Application.Notifications.GroupChat;
using ChatApp.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Api.Handlers.GroupChat
{
    public class UserLeavedGroupHandler : INotificationHandler<UserLeavedGroupNotification>
    {
        private readonly IHubContext<ChatHub> _hubContext;
        public UserLeavedGroupHandler(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }
        public async Task Handle(UserLeavedGroupNotification n, CancellationToken cancellationToken)
        {
            await _hubContext.Clients.Client(n.UserId.ToString()).SendAsync("ReceiveStatus", "Opuściłeś czat!");
            await _hubContext.Clients.User(n.UserId.ToString()).SendAsync("RequestLeaveGroupSignalR", n.ChatId);
            await Task.WhenAll(
            _hubContext.Clients.Group(n.ChatId.ToString()).SendAsync("ReceiveMessage", n.SystemMessage),
            _hubContext.Clients.Group(n.ChatId.ToString()).SendAsync("UsersInChatReload", n.ChatId)
                );
        }
    }
}
