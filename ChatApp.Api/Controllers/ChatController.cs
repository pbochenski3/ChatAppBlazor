using Azure.Core;
using ChatApp.Application.DTO;
using ChatApp.Application.DTO.Chats;
using ChatApp.Application.DTO.Requests;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Interfaces.Chats;
using ChatApp.Application.Interfaces.Service;
using ChatApp.Application.Notifications;
using ChatApp.Application.Services;
using ChatApp.Application.Services.Chats;
using ChatApp.Domain.Enums;
using ChatApp.Domain.Models;
using ChatApp.Domain.Shared;
using MediatR;
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
        private readonly IChatService _chatService;
        private readonly IFileService _fileService;
        private readonly IUserChatService _userChatService;

        public ChatController(IChatService chatService, IFileService fileService, IUserChatService userChatService)
        {
            _chatService = chatService;
            _userChatService = userChatService;
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
                string imageUrl = await _fileService.SaveImageAsync(file, UploadType.ChatImage, chatId, userId);
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
                string avatarUrl = await _fileService.SaveImageAsync(file, UploadType.GroupAvatar, chatId);
                await _chatService.UpdateGroupAvatarUrl(chatId, avatarUrl);
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
                var isArchive = await _chatService.IsChatArchive(chatId);
                if(isArchive == true)
                {
                    return BadRequest();
                }
                await _chatService.DeleteChatAsync(chatId, userId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPatch("{chatId}/name")]
        public async Task<IActionResult> UpdateChatNameAsync([FromRoute] Guid chatId, [FromBody] ChangeChatNameRequest request)
        {
            var userId = CurrentUserId;
            if (chatId == Guid.Empty) return BadRequest();
            var isArchive = await _chatService.IsChatArchive(chatId);
            if(isArchive == true)
            {
                return BadRequest();
            }
            try
            {
                await _userChatService.UpdateChatNameAsync(chatId, request);
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
