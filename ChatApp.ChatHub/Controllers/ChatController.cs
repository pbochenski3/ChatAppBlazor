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

    public class ChatController : AppControllerBase
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
