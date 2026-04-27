using ChatApp.Application.Notifications.Chat;
using ChatApp.Domain.Enums;
using ChatApp.Domain.Interfaces.Repository;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Api.Handlers.Chat
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
        public async Task Handle(ChatNameUpdatedNotification n, CancellationToken cancellationToken)
        {
            var usersInChat = await _userChatRepo.GetUsersInChatIdAsync(n.ChatId);
            var usersToNofitify = usersInChat.Select(u => u.ToString()).ToList();
            var systemMessage = new Domain.Models.Message
            {
                ChatID = n.ChatId,
                MessageID = Guid.CreateVersion7(),
                Content = $"{n.Request.AdminName} zmienił nazwe czatu na {n.Request.NewName}.",
                MessageType = MessageType.System,
                SentAt = DateTime.UtcNow,
            };
            await _messageRepo.AddMessageAsync(systemMessage);
            await _hubContext.Clients.Users(usersToNofitify).SendAsync("ReceiveMessage", systemMessage);
            await _hubContext.Clients.Users(usersToNofitify).SendAsync("UpdateChatName", n.ChatId, n.Request.NewName);
        }
    }
}
