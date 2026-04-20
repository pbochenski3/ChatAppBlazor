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

namespace ChatApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessageController : AppControllerBase
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
        [HttpPost]
        public async Task<IActionResult> SendChatMessageAsync([FromBody] MessageDTO dto)
        {
            try
            {
                var userId = CurrentUserId;
                if (dto.ChatID == Guid.Empty) return BadRequest();

                await _messageService.SaveMessageAsync(dto);
                return Ok();
            }
            catch (Exception ex)
            {
                {
                    return BadRequest(ex.Message);
                }
            }
        }
        [HttpGet("{chatId}/history")]
        public async Task<ActionResult<List<MessageDTO>>> GetChatMessageHistoryAsync([FromRoute] Guid chatId, CancellationToken ct)
        {
            var userId = CurrentUserId;
            if (chatId == Guid.Empty) return BadRequest();
            List<MessageDTO> messages = await _messageService.GetChatMessageHistoryAsync(userId, chatId, ct);

            return Ok(messages);
        }
        [HttpPatch("chat/{chatId}/read/{messageId}")]
        public async Task<IActionResult> MarkMessageAsReadAsync([FromRoute] Guid chatId, [FromRoute] Guid messageId)
        {
            var userId = CurrentUserId;
            if (chatId == Guid.Empty) return BadRequest();
            await _readStatusService.MarkMessageAsReadAsync(userId, chatId,messageId);
            return Ok();
        }
        [HttpPatch("chat/{chatId}/read-all")]
        public async Task<IActionResult> MarkAllMessagesAsReadAsync([FromRoute] Guid chatId,CancellationToken ct)
        {
            var userId = CurrentUserId;
            if (chatId == Guid.Empty) return BadRequest();
            await _readStatusService.MarkAllMessagesAsReadAsync(userId, chatId, ct);
            return Ok();
        }
    }
}

