using ChatApp.Application.DTO;
using ChatApp.Application.DTO.Chats;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Interfaces.Chats;
using ChatApp.Application.Interfaces.Service;
using ChatApp.Domain.Enums;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace ChatApp.ChatHub
{
    public class ChatHub : Hub
    {
        private readonly IUserService _userService;
        private readonly ISidebarService _sidebarService;
        private readonly ILogger<ChatHub> _logger;
    

        protected Guid UserId => Guid.TryParse(Context.UserIdentifier, out var parseId) ? parseId : Guid.Empty;

        public ChatHub(
            ILogger<ChatHub> logger,
            IUserService userService,
            ISidebarService sidebarService

            )
        {
            _logger = logger;
            _userService = userService;
            _sidebarService = sidebarService;
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
                _logger.LogDebug(ex, "Failed to log JoinChatGroupSignalAsync");
            }
        }
  
        public async Task<string> GetUserAvatarUrlAsync()
        {
            return await _userService.GetAvatarUrlAsync(UserId);
        }

        //public async Task<List<UserChatDTO>> GetUserChatListAsync()
        //{
        //    return await _userChatService.GetUserChatListAsync(UserId);
        //}
        public async Task<List<UserChatDTO>> GetSidebarItemsAsync()
        {
            return await _sidebarService.GetSidebarItemsAsync(UserId);
        }
    } 
}
