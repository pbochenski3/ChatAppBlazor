using ChatApp.Application.Notifications.Chat;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Interfaces.Repository;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Api.NotificationHandlers.Chat
{
    public class ChatNameUpdatedHandler : INotificationHandler<ChatNameUpdatedNotification>
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IUserChatRepository _userChatRepo;
        private readonly IMessageRepository _messageRepo;
        public ChatNameUpdatedHandler(IMessageRepository messageRepo, IUserChatRepository userChatRepo, IHubContext<ChatHub> hubContext)
        {
            _messageRepo = messageRepo;
            _userChatRepo = userChatRepo;
            _hubContext = hubContext;

        }
        public async Task Handle(ChatNameUpdatedNotification n, CancellationToken ct)
        {
            var usersInChat = await _userChatRepo.GetUsersInChatIdAsync(n.ChatId);
            var usersToNofitify = usersInChat.Select(u => u.ToString()).ToList();
            var systemMessage = Message.CreateSystemMessage(n.ChatId, $"{n.Request.AdminName} zmienił nazwe czatu na {n.Request.NewName}");
            Guid userIdDummy = Guid.Empty;
            await _messageRepo.AddMessageAsync(systemMessage);
            await _hubContext.Clients.Users(usersToNofitify).SendAsync("ReceiveMessage", systemMessage);
            await _hubContext.Clients.Users(usersToNofitify).SendAsync("UpdateChatName", n.ChatId, n.Request.NewName, userIdDummy);
        }
    }
}
