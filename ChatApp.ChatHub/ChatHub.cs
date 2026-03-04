using Microsoft.AspNetCore.SignalR;

namespace ChatApp.ChatHub
{
    public class ChatHub : Hub
    {
        private readonly ILogger<ChatHub> _logger;
        public string  name { get; set; }
        public string message { get; set; }
        public ChatHub(ILogger<ChatHub> logger) 
        {
            _logger = logger;
        }
        public override Task OnConnectedAsync()
        {
            _logger.LogInformation($"User {Context.ConnectionId} connected successfully");
            return base.OnConnectedAsync();
        }
        public async Task Send(string name, string message)

        {
            Clients.All.SendAsync(name, message).Wait();
        }
    }
}
