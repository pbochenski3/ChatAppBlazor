using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Api
{
    public class ChatHub : Hub
    {
        private readonly ILogger<ChatHub> _logger;


        protected Guid UserId => Guid.TryParse(Context.UserIdentifier, out var parseId) ? parseId : Guid.Empty;

        public ChatHub(ILogger<ChatHub> logger)
        {
            _logger = logger;
        }

        public override Task OnConnectedAsync()
        {
            try
            {
                _logger.LogInformation("Hub connected: ConnectionId={ConnectionId}, UserIdentifier={UserIdentifier}", Context.ConnectionId, Context.UserIdentifier);
                var nameClaim = Context.User?.Identity?.Name;
                _logger.LogDebug("Connected user name claim: {NameClaim}", nameClaim);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error while logging OnConnectedAsync");
            }

            return base.OnConnectedAsync();
        }
        public async Task RemoveMeFromChatGroup(Guid chatId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId.ToString());
            _logger.LogInformation("Connection {ConnId} left group {ChatId}", Context.ConnectionId, chatId);
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            try
            {
                _logger.LogInformation("Hub disconnected: ConnectionId={ConnectionId}, UserIdentifier={UserIdentifier}, Reason={Reason}", Context.ConnectionId, Context.UserIdentifier, exception?.Message);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error while logging OnDisconnectedAsync");
            }

            return base.OnDisconnectedAsync(exception);
        }

        public async Task JoinChatGroupSignalAsync(Guid chatId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatId.ToString());
            try
            {
                _logger.LogInformation("Connection {ConnectionId} (UserId={UserId}) joined group {ChatId}", Context.ConnectionId, Context.UserIdentifier, chatId);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Failed to add user to group");
            }
        }

        //public async Task<string> GetUserAvatarUrlAsync()
        //{
        //    return await _userService.GetAvatarUrlAsync(UserId);
        //}

        //public async Task<List<UserChatDTO>> GetUserChatListAsync()
        //{
        //    return await _userChatService.GetUserChatListAsync(UserId);
        //}
    }
}
