using ChatApp.Application.Interfaces.Chats;
using ChatApp.Application.Services.Chats;
using ChatApp.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.ChatHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GroupChatController : AppControllerBase
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IGroupChatService _groupChatService;


        public GroupChatController(IHubContext<ChatHub> hubContext, IGroupChatService groupChatService)
        {
            _hubContext = hubContext;
            _groupChatService = groupChatService;
        }
        [HttpPost("from-private-chat/{chatId}")]
        public async Task<IActionResult> CreateGroupChatAsync([FromBody] HashSet<Guid> usersToAdd, [FromRoute] Guid chatId, CancellationToken ct)
        {
            try
            {
                await _groupChatService.CreateGroupChatAsync(chatId, usersToAdd);
                var usersToNotify = usersToAdd.Select(id => id.ToString()).ToList();
                await _hubContext.Clients.Users(usersToNotify).SendAsync("SideBarReload", true);
                await _hubContext.Clients.Group(chatId.ToString()).SendAsync("SideBarReload", true);
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
                var usersToNotify = usersToAdd.Select(id => id.ToString()).ToList();
                await _hubContext.Clients.Users(usersToNotify).SendAsync("SideBarReload", true);
                await _hubContext.Clients.Group(actionResult.GroupChatId.ToString()).SendAsync("ReceiveMessage", actionResult.SystemMessage);
                await _hubContext.Clients.Group(actionResult.GroupChatId.ToString()).SendAsync("UsersInChatReload", actionResult.GroupChatId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("{chatId}/remove-user")]
        public async Task<IActionResult> LeaveGroupChatAsync(Guid chatId, string username)
        {
            var userId = CurrentUserId;
            try
            {
                var message = await _groupChatService.ProcessLeaveGroupChatAsync(chatId, userId, username);
                await _hubContext.Clients.Group(chatId.ToString()).SendAsync("ReceiveMessage", message);
                await _hubContext.Clients.Group(chatId.ToString()).SendAsync("UsersInChatReload", chatId);
                await _hubContext.Clients.Client(userId.ToString()).SendAsync("ChatReload", chatId, true);
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
