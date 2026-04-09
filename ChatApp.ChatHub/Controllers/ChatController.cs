using ChatApp.Application.DTO;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Interfaces.Chats;
using ChatApp.Application.Interfaces.Service;
using ChatApp.Application.Services.Chats;
using ChatApp.Domain.Enums;
using ChatApp.Domain.Shared;
using ChatApp.Domain.Models;
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
        private readonly IMessageService _messageService;
        private readonly IFileService _fileService;
        private readonly IChatReadStatusService _readStatusService;

        public ChatController(IChatService chatService, IHubContext<ChatHub> hubContext, IFileService fileService,IMessageService messageService,IChatReadStatusService readStatusService)
        {
            _chatService = chatService;
            _messageService = messageService;
            _hubContext = hubContext;
            _fileService = fileService;
            _readStatusService = readStatusService;
        }
        [Authorize]
        [HttpPost("sendMessage")]
        public async Task<IActionResult> SendChatMessageAsync([FromBody] MessageDTO dto)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
                {
                    return Unauthorized();
                }
                if (dto.ChatID == Guid.Empty) return BadRequest();

                await _messageService.SaveMessageAsync(dto);
                await _readStatusService.SaveLastSentMessageIdAsync(dto.ChatID, dto.MessageID);
                await _hubContext.Clients.Group(dto.ChatID.ToString()).SendAsync("ReceiveMessage", dto);
                var chat = await _chatService.GetUsersInChatIdAsync(dto.ChatID);
                var participants = chat.Select(id => id.ToString()).ToList();
                await _hubContext.Clients.Users(participants).SendAsync("UpdateLastMessage", dto.ChatID, dto.SenderUsername, dto.Content);
                return Ok();
            }
            catch (Exception ex)
            {
                {
                    return BadRequest(ex.Message);
                }
            }
        }
        [Authorize]
        [HttpPost("saveChatImage")]
        public async Task<IActionResult> SaveChatImageAsync(IFormFile file, [FromQuery] Guid chatId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
            {
                return Unauthorized();
            }
            if (chatId == Guid.Empty) return BadRequest();
            try
            {
                string imageUrl = await SaveImageAsync(file, UploadType.ChatImage, chatId, userGuid);
                var response = new FileResponse { Url = imageUrl };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
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
                string avatarUrl = await SaveImageAsync(file, UploadType.GroupAvatar, chatId);
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
        public async Task<string> SaveImageAsync(IFormFile file,UploadType type, Guid? chatId = null, Guid? userId = null)
        {
            var url = string.Empty;
            var extension = Path.GetExtension(file.FileName).ToLower();
            using var stream = file.OpenReadStream();
            switch(type)
            {
                case UploadType.GroupAvatar:
                    url = await _fileService.SaveAvatar(stream, extension, type);
                    break;
                case UploadType.ChatImage:
                    url = await _fileService.SaveChatImage(stream, extension, chatId, userId);
                    break;
            }
            return url;
        }
    }
}
