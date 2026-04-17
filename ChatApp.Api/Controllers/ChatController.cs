using Azure.Core;
using ChatApp.Application.DTO;
using ChatApp.Application.DTO.Chats;
using ChatApp.Application.DTO.Requests;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Interfaces.Chats;
using ChatApp.Application.Interfaces.Service;
using ChatApp.Application.Services;
using ChatApp.Application.Services.Chats;
using ChatApp.Domain.Enums;
using ChatApp.Domain.Models;
using ChatApp.Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace ChatApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class ChatController : AppControllerBase
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IChatService _chatService;
        private readonly IFileService _fileService;
        private readonly IUserChatService _userChatService;
        private readonly IMessageService _messageService;

        public ChatController(IChatService chatService, IHubContext<ChatHub> hubContext, IFileService fileService, IUserChatService userChatService, IMessageService messageService)
        {
            _chatService = chatService;
            _userChatService = userChatService;
            _messageService = messageService;
            _hubContext = hubContext;
            _fileService = fileService;
        }
        
        [HttpGet("{chatId}/details")]
        public async Task<ActionResult<UserChatDTO>> GetChatDetailsAsync([FromRoute] Guid chatId, CancellationToken ct)
        {
            var userId = CurrentUserId;
            if (chatId == Guid.Empty) return BadRequest();
            var chatDetails = await _userChatService.GetUserChatDetailsAsync(chatId, userId, ct);
            if (chatDetails == null) return NotFound();
            return Ok(chatDetails);
        }
        [HttpPost("chatImage")]
        public async Task<IActionResult> SaveChatImageAsync(IFormFile file, [FromQuery] Guid chatId)
        {
            var userId = CurrentUserId;
            if (chatId == Guid.Empty) return BadRequest();
            try
            {
                string imageUrl = await SaveImageAsync(file, UploadType.ChatImage, chatId, userId);
                var response = new FileResponse { Url = imageUrl };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("groupAvatar")]
        public async Task<IActionResult> UploadGroupAvatarAsync(IFormFile file, [FromQuery] Guid chatId)
        {
            var userId = CurrentUserId;
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
        [HttpDelete("{chatId}")]
        public async Task<IActionResult> DeleteChatAsync([FromRoute] Guid chatId)
        {
            var userId = CurrentUserId;
            if (chatId == Guid.Empty) return BadRequest();
            try
            {
                await _chatService.DeleteChatAsync(chatId, userId);
                string user = userId.ToString();
                await _hubContext.Clients.User(user).SendAsync("ReceiveStatus", "Czat został usunięty!");
                await _hubContext.Clients.User(user).SendAsync("SidebarChatsReload");
                await _hubContext.Clients.User(user).SendAsync("ChatClose");
                return Ok();
            }
            catch (Exception ex)
            {
                string user = userId.ToString();
                await _hubContext.Clients.User(user).SendAsync("ReceiveStatus", $"Błąd podczas usuwania czatu: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }
        [HttpPatch("{chatId}/name")]
        public async Task<IActionResult> UpdateChatNameAsync([FromRoute] Guid chatId, [FromBody] ChangeChatNameRequest request)
        {
            var userId = CurrentUserId;
            if (chatId == Guid.Empty) return BadRequest();
            try
            {
                await _userChatService.UpdateChatNameAsync(chatId, request.NewName);
                var usersInChat = await _chatService.GetUsersInChatIdAsync(chatId);
                var usersToNofitify = usersInChat.Select(u => u.ToString()).ToList();
                var systemMessage = new MessageDTO
                {
                    ChatID = chatId,
                    MessageID = Guid.CreateVersion7(),
                    Content = $"{request.AdminName} zmienił nazwe czatu na {request.NewName}.",
                    SenderUsername = "SYSTEM",
                    MessageType = MessageType.System,
                    SentAt = DateTime.UtcNow,
                };
                await _messageService.SaveMessageAsync(systemMessage);
                await _hubContext.Clients.Users(usersToNofitify).SendAsync("ReceiveMessage", systemMessage);
                await _hubContext.Clients.Users(usersToNofitify).SendAsync("UpdateChatName", chatId, request.NewName);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("{chatId}/existing")]
        public async Task<IActionResult> CheckGroupChatExistsAsync([FromRoute] Guid chatId, CancellationToken ct)
        {
            var userId = CurrentUserId;
            if (chatId == Guid.Empty) return BadRequest();
            var exists = await _chatService.IsChatExistingAsync(chatId, userId);
            return Ok(exists);
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
        [HttpGet("{chatId}/usersId")]
        public async Task<IActionResult> GetChatUsersAsync([FromRoute] Guid chatId, CancellationToken ct)
        {
            try
            {
                var ids = await _chatService.GetUsersInChatIdAsync(chatId);

                return Ok(ids);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}
