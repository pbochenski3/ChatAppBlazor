using ChatApp.Application.Interfaces;
using ChatApp.Application.Interfaces.Chats;
using ChatApp.Application.Services.Chats;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace ChatApp.ChatHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class ChatController : ControllerBase
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IChatService _chatService;
        private readonly IFileService _fileService;

        public ChatController(IChatService chatService, IHubContext<ChatHub> hubContext, IFileService fileService)
        {
            _chatService = chatService;
            _hubContext = hubContext;
            _fileService = fileService;
        }
        [Authorize]
        [HttpPost("updateGroupAvatar")]
        public async Task<IActionResult> UploadGroupAvatarAsync(IFormFile file, [FromQuery] Guid chatId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
            {
                return Unauthorized();
            }
            if (chatId == Guid.Empty) return BadRequest();
            try
            {
                var extension = Path.GetExtension(file.FileName).ToLower();
                using var stream = file.OpenReadStream();
                string avatarUrl = await _fileService.SaveUserAvatarAsync(stream, extension);
                await _chatService.UpdateGroupAvatarUrl(chatId, avatarUrl);
                var usersInChat = await _chatService.GetUsersInChatIdAsync(chatId);
                var usersToNofitify = usersInChat.Select(u => u.ToString()).ToList();
                await _hubContext.Clients.Users(usersToNofitify).SendAsync("GroupAvatarReload",avatarUrl, chatId);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
