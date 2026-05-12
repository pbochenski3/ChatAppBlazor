using ChatApp.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Api.SignalR
{
    public class SignalRNotificationService : ISignalRNotificationService
    {
        private readonly ILogger<SignalRNotificationService> _logger;
        private readonly IHubContext<ChatHub> _hubContext;

        public SignalRNotificationService(IHubContext<ChatHub> hubContext, ILogger<SignalRNotificationService> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }
        public async Task SendToUser(string user, string methodName, params object[] args)
        {
            await _hubContext.Clients.User(user).SendCoreAsync(methodName, args);
            _logger.LogInformation("Sent {methodName} to user {user} with args: {args}", methodName, user, args);
        }
        public async Task SendToUsers(List<string> users, string methodName, params object[] args)
        {
            await _hubContext.Clients.Users(users).SendCoreAsync(methodName, args);
            _logger.LogInformation("Sent {methodName} to users {users} with args: {args}", methodName, users, args);
        }
        public async Task SendToGroup(string group, string methodName, params object[] args)
        {
            await _hubContext.Clients.Group(group).SendCoreAsync(methodName, args);
            _logger.LogInformation("Sent {methodName} to group {group} with args: {args}", methodName, group, args);
        }

    }
}
