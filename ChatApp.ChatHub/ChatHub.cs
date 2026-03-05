using ChatApp.Application.DTO;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Services;
using ChatApp.Domain.Models;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.ChatHub
{
    public class ChatHub : Hub
    {
        private readonly IMessageService _messageService;
        private readonly IUserService _userService;
        private readonly ILogger<ChatHub> _logger;
        public ChatHub(ILogger<ChatHub> logger, IMessageService messageService, IUserService userService)
        {
            _logger = logger;
            _messageService = messageService;
            _userService = userService;
        }
        public override Task OnConnectedAsync()
        {
            _logger.LogInformation($"User {Context.ConnectionId} connected successfully");
            return base.OnConnectedAsync();
        }
        public async Task SendMessage(MessageDTO dto)
        {
            await _messageService.SendMessageAsync(dto);
            await Clients.All.SendAsync("ReceiveMessage", dto.SenderName, dto.Content);
        }
        public async Task SendMessageToUsers(string receiverConnectionId, string senderUsername, string message)
        {
            await Clients.Client(receiverConnectionId).SendAsync("ReceiveMessage", senderUsername, message);
        }
        public async Task SendUser(UserDTO dto)
        {
            try
            {
                await _userService.Register(dto);
                await Clients.Caller.SendAsync("ReceiveRegistrationStatus", "User registered successfully.");
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("ReceiveRegistrationStatus", $"{ex.Message}");
            }
        }
    }
}
