using ChatApp.Application.DTO;
using ChatApp.Application.Interfaces;
using ChatApp.Domain.Models;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.ChatHub
{
    public class ChatHub : Hub
    {
        private readonly IMessageRepository _messageRepo;
        private readonly ILogger<ChatHub> _logger;
        public ChatHub(ILogger<ChatHub> logger, IMessageRepository messageRepo) 
        {
            _logger = logger;
            _messageRepo = messageRepo;
        }
        public override Task OnConnectedAsync()
        {
            _logger.LogInformation($"User {Context.ConnectionId} connected successfully");
            return base.OnConnectedAsync();
        }
        public async Task SendMessage(MessageDTO dto)
        {
            var message = new Message
            {
                Content = dto.Content,
                SentAt = DateTime.UtcNow,
                SenderID = dto.SenderID,
                ChatID = dto.ChatID
            };
            await _messageRepo.AddAsync(message);
            await _messageRepo.SaveChangesAsync();
            await Clients.All.SendAsync("ReceiveMessage", dto.SenderName, dto.Content);
        }
        public async Task SendMessageToUsers(string receiverConnectionId, string senderUsername, string message)
        {
            await Clients.Client(receiverConnectionId).SendAsync("ReceiveMessage", senderUsername, message);
        }
    }
}
