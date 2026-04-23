using ChatApp.Application.Notifications.Chat;
using ChatApp.Domain.Repository;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Api.Handlers.Chat
{
    public class GroupAvatarUpdatedHandler : INotificationHandler<GroupAvatarUpdatedNotification>
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IUserChatRepository _userChatRepo;

        public GroupAvatarUpdatedHandler(IHubContext<ChatHub> hubContext, IUserChatRepository userChatRepo)
        {
            _hubContext = hubContext;
            _userChatRepo = userChatRepo;
        }

        public async Task Handle(GroupAvatarUpdatedNotification n, CancellationToken ct)
        {
            var usersInChat = await _userChatRepo.GetUsersInChatIdAsync(n.ChatId);
            var usersToNotify = usersInChat.Select(u => u.ToString()).ToList();

            await _hubContext.Clients.Users(usersToNotify)
                .SendAsync("GroupAvatarReload", n.AvatarUrl, n.ChatId, ct);
        }
    }
}
