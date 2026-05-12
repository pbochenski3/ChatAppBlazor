using ChatApp.Application.Interfaces;
using ChatApp.Application.Notifications.Chat;
using ChatApp.Domain.Interfaces.Repository;
using MediatR;

namespace ChatApp.Api.NotificationHandlers.Chat
{
    public class ChatUserAliasChangedHandler : INotificationHandler<ChatUserAliasChangedNotification>
    {
        private readonly ISignalRNotificationService _signalR;
        private readonly IMessageRepository _messageRepo;
        private readonly IUserChatRepository _userChatRepo;
        private readonly IChatRepository _chatRepo;
        public ChatUserAliasChangedHandler(IUserChatRepository userChatRepo, IMessageRepository messageRepo, IChatRepository chatRepo, ISignalRNotificationService signalR)
        {
            _messageRepo = messageRepo;
            _userChatRepo = userChatRepo;
            _chatRepo = chatRepo;
            _signalR = signalR;
        }

        public async Task Handle(ChatUserAliasChangedNotification n, CancellationToken cancellationToken)
        {
            var isGroup = await _chatRepo.IsChatGroupAsync(n.ChatId);
            var usersInChat = await _userChatRepo.GetUsersInChatIdAsync(n.ChatId);
            var usersToNofitify = usersInChat.Select(u => u.ToString()).ToList();
            var systemMessage = Domain.Entities.Message.CreateSystemMessage(n.ChatId, $"{n.Request.adminName} zmienił nazwe użytkownika {n.Request.username} na {n.Request.Alias} !");
            await _messageRepo.AddMessageAsync(systemMessage);
            await _signalR.SendToUsers(usersToNofitify, "ReceiveMessage", systemMessage);

            if (!isGroup)
            {
                await _signalR.SendToUsers(usersToNofitify, "UpdateChatName", n.ChatId, n.Request.Alias, n.Request.changeUserId);

            }

            await _signalR.SendToUsers(usersToNofitify, "UserAliasChanged", n.ChatId, n.Request.changeUserId, n.Request.Alias);
            await _signalR.SendToUsers(usersToNofitify, "UsersInChatReload", n.ChatId);
        }
    }
}
