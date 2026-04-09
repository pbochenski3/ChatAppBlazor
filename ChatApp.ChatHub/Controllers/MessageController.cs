using ChatApp.Application.DTO;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Interfaces.Chats;
using ChatApp.Application.Interfaces.Service;
using ChatApp.Application.Services;
using ChatApp.Application.Services.Chats;
using ChatApp.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ChatApp.ChatHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessageController : Controller
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IChatService _chatService;
        private readonly IMessageService _messageService;
        private readonly IChatReadStatusService _readStatusService;
        public MessageController(IChatService chatService, IHubContext<ChatHub> hubContext, IMessageService messageService, IChatReadStatusService readStatusService)
        {
            _hubContext = hubContext;
            _messageService = messageService;
            _readStatusService = readStatusService;
            _chatService = chatService;
        }
        //[Authorize]
        //[HttpGet]
        //public async Task<IActionResult> GetUnreadMessageCountAsync(Guid chatId, CancellationToken c)
        //{
        //    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
        //    {
        //        return Unauthorized();
        //    }
        //    await _readStatusService.MarkChatMessagesAsReadAsync(userGuid, chatId,ct);
        //}
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
    }
}
