using Azure.Core;
using ChatApp.Application.DTO;
using ChatApp.Application.Interfaces.Chats;
using ChatApp.Application.Interfaces.Service;
using ChatApp.Application.Notifications.Chat;
using ChatApp.Application.Services;
using ChatApp.Application.Services.Chats;
using ChatApp.Domain.Enums;
using ChatApp.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Api.Handlers.Chat
{
    public class ChatNameUpdatedHandler : INotificationHandler<ChatNameUpdatedNotification>
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IChatService _chatService;
        private readonly IMessageService _messageService;
        public ChatNameUpdatedHandler(IMessageService messageService, IChatService chatService,IHubContext<ChatHub> hubContext)
        {
            _messageService = messageService;
            _chatService = chatService;
            _hubContext = hubContext;

        }
        public async Task Handle(ChatNameUpdatedNotification n, CancellationToken cancellationToken)
        {
            var usersInChat = await _chatService.GetUsersInChatIdAsync(n.ChatId);
            var usersToNofitify = usersInChat.Select(u => u.ToString()).ToList();
            var systemMessage = new MessageDTO
            {
                ChatID = n.ChatId,
                MessageID = Guid.CreateVersion7(),
                Content = $"{n.Request.AdminName} zmienił nazwe czatu na {n.Request.NewName}.",
                SenderUsername = "SYSTEM",
                MessageType = MessageType.System,
                SentAt = DateTime.UtcNow,
            };
            await _messageService.SaveMessageAsync(systemMessage);
            await _hubContext.Clients.Users(usersToNofitify).SendAsync("ReceiveMessage", systemMessage);
            await _hubContext.Clients.Users(usersToNofitify).SendAsync("UpdateChatName", n.ChatId, n.Request.NewName);
        }
    }
}
