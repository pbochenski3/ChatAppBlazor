using ChatApp.Application.Interfaces.Chats;
using ChatApp.Application.Services.Chats;
using ChatApp.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace ChatApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GroupChatController : AppControllerBase
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IGroupChatService _groupChatService;
        private readonly ILogger<GroupChatController> _logger;


        public GroupChatController(IHubContext<ChatHub> hubContext, IGroupChatService groupChatService, ILogger<GroupChatController> logger)
        {
            _hubContext = hubContext;
            _groupChatService = groupChatService;
            _logger = logger;
        }
        [HttpPost("from-private-chat/{chatId}")]
        public async Task<IActionResult> CreateGroupChatAsync([FromBody] HashSet<Guid> usersToAdd, [FromRoute] Guid chatId, CancellationToken ct)
        {
            try
            {
                _logger.LogInformation("CreateGroupChatAsync called by user {UserId} for existingChatId={ChatId} with {Count} usersToAdd", CurrentUserId, chatId, usersToAdd?.Count ?? 0);
                var newChatId = await _groupChatService.CreateGroupChatAsync(chatId, usersToAdd);
                _logger.LogInformation("New group chat created: {NewChatId}", newChatId);

                var usersInNewChat = await _groupChatService.ProccesGetChatUsersAsync(newChatId);
                var usersToNotify = usersInNewChat.Select(u => u.UserID.ToString()).ToList();
                _logger.LogDebug("Users to notify for new chat {NewChatId}: {Users}", newChatId, string.Join(',', usersToNotify));

                if (usersToNotify.Any())
                {
                    await _hubContext.Clients.Users(usersToNotify).SendAsync("SidebarChatsReload");
                }

                await _hubContext.Clients.Group(newChatId.ToString()).SendAsync("SidebarChatsReload");
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("{chatId}/add-users")]
        public async Task<IActionResult> AddUsersToGroupChatAsync([FromBody] HashSet<Guid> usersToAdd, [FromRoute] Guid chatId, CancellationToken ct)
        {
            var userId = CurrentUserId;
            try
            {
                var actionResult = await _groupChatService.ProcessAddToGroupChatAsync(chatId, usersToAdd, userId);
                var usersToNotify = actionResult.UsersInChat.Select(id => id.ToString()).ToList();

                if (usersToNotify.Any())
                {
                    await _hubContext.Clients.Users(usersToNotify).SendAsync("SidebarChatsReload");
                    await _hubContext.Clients.Users(usersToNotify).SendAsync("ChatReload",chatId, true);
                }

                await _hubContext.Clients.Group(actionResult.GroupChatId.ToString()).SendAsync("ReceiveMessage", actionResult.SystemMessage);
                await _hubContext.Clients.Group(actionResult.GroupChatId.ToString()).SendAsync("UsersInChatReload", actionResult.GroupChatId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("{chatId}/{username}")]
        public async Task<IActionResult> LeaveGroupChatAsync(Guid chatId, string username)
        {
            var userId = CurrentUserId;
            try
            {
                var message = await _groupChatService.ProcessLeaveGroupChatAsync(chatId, userId, username);
                await _hubContext.Clients.User(userId.ToString()).SendAsync("CloseConnection");
                await Task.WhenAll(
                _hubContext.Clients.Group(chatId.ToString()).SendAsync("ReceiveMessage", message),
                _hubContext.Clients.Group(chatId.ToString()).SendAsync("UsersInChatReload", chatId)
                    );

                //to pozneij do przeniesienia
                await _hubContext.Clients.Client(userId.ToString()).SendAsync("ReceiveStatus", "Opuściłeś czat!");

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("{chatId}/users")]
        public async Task<IActionResult> GetChatUsersAsync([FromRoute] Guid chatId, CancellationToken ct)
        {
            try
            {
                var users = await _groupChatService.ProccesGetChatUsersAsync(chatId);
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
