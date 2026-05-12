using ChatApp.Application.Interfaces;
using ChatApp.Application.Notifications.Chat;
using ChatApp.Domain.Interfaces.Repository;
using MediatR;

namespace ChatApp.Api.NotificationHandlers.Chat
{
    public class GroupAvatarUpdatedHandler : INotificationHandler<GroupAvatarUpdatedNotification>
    {
        private readonly IUserChatRepository _userChatRepo;
        private readonly ISignalRNotificationService _signalR;

        public GroupAvatarUpdatedHandler(IUserChatRepository userChatRepo, ISignalRNotificationService signalR)
        {
            _userChatRepo = userChatRepo;
            _signalR = signalR;
        }

        public async Task Handle(GroupAvatarUpdatedNotification n, CancellationToken ct)
        {
            var usersInChat = await _userChatRepo.GetUsersInChatIdAsync(n.ChatId);
            var usersToNotify = usersInChat.Select(u => u.ToString()).ToList();

            await _signalR.SendToUsers(usersToNotify, "GroupAvatarReload", n.AvatarUrl, n.ChatId);
        }
    }
}
