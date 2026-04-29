using ChatApp.Application.Notifications.Chat;
using ChatApp.Domain.Enums;
using ChatApp.Domain.Interfaces.Repository;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Api.Handlers.Chat
{
    public class ChatUserAliasChangedHandler : INotificationHandler<ChatUserAliasChangedNotification>
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IMessageRepository _messageRepo;
        private readonly IUserChatRepository _userChatRepo;
        public ChatUserAliasChangedHandler(IHubContext<ChatHub> hubContext,IUserChatRepository userChatRepo,IMessageRepository messageRepo)
        {
            _messageRepo = messageRepo;
            _hubContext = hubContext;
            _userChatRepo = userChatRepo;
        }
        public async Task Handle(ChatUserAliasChangedNotification n, CancellationToken cancellationToken)
        {
            var usersInChat = await _userChatRepo.GetUsersInChatIdAsync(n.ChatId);
            var usersToNofitify = usersInChat.Select(u => u.ToString()).ToList();
            var systemMessage = new Domain.Models.Message
            {
                ChatID = n.ChatId,
                MessageID = Guid.CreateVersion7(),
                Content = $"{n.Request.adminName} zmienił nazwe użytkownika {n.Request.username} na {n.Request.Alias} !",
                MessageType = MessageType.System,
                SentAt = DateTime.UtcNow,
            };
            await _messageRepo.AddMessageAsync(systemMessage);
            await _hubContext.Clients.Users(usersToNofitify).SendAsync("ReceiveMessage", systemMessage);
            await _hubContext.Clients.Users(usersToNofitify).SendAsync("UpdateChatName", n.ChatId, n.Request.Alias);
        }
    }
}
