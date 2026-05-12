using ChatApp.Application.Interfaces;
using ChatApp.Application.Notifications.Chat;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Interfaces.Repository;
using MediatR;

namespace ChatApp.Api.NotificationHandlers.Chat
{
    public class ChatNameUpdatedHandler : INotificationHandler<ChatNameUpdatedNotification>
    {
        private readonly ISignalRNotificationService _signalR;
        private readonly IUserChatRepository _userChatRepo;
        private readonly IMessageRepository _messageRepo;
        public ChatNameUpdatedHandler(IMessageRepository messageRepo, IUserChatRepository userChatRepo, ISignalRNotificationService signalR)
        {
            _messageRepo = messageRepo;
            _userChatRepo = userChatRepo;
            _signalR = signalR;

        }
        public async Task Handle(ChatNameUpdatedNotification n, CancellationToken ct)
        {
            var usersInChat = await _userChatRepo.GetUsersInChatIdAsync(n.ChatId);
            var usersToNofitify = usersInChat.Select(u => u.ToString()).ToList();
            var systemMessage = Message.CreateSystemMessage(n.ChatId, $"{n.Request.AdminName} zmienił nazwe czatu na {n.Request.NewName}");
            Guid userIdDummy = Guid.Empty;
            await _messageRepo.AddMessageAsync(systemMessage);
            await _signalR.SendToUsers(usersToNofitify, "ReceiveMessage", systemMessage);
            await _signalR.SendToUsers(usersToNofitify, "UpdateChatName", n.ChatId, n.Request.NewName, userIdDummy);
        }
    }
}
