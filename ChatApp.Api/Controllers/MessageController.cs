using ChatApp.Application.DTO;
using ChatApp.Application.Feature.Chat;
using ChatApp.Application.Feature.Chat.GetChatMessageHistory;
using ChatApp.Application.Feature.Chat.MarkAllMessagesAsRead;
using ChatApp.Application.Feature.Chat.MarkAsRead;
using ChatApp.Application.Feature.Chat.SendChatMessage;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Interfaces.Chats;
using ChatApp.Application.Interfaces.Service;
using ChatApp.Application.Services;
using ChatApp.Application.Services.Chats;
using ChatApp.Domain.Models;
using MediatR;
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
        private readonly IMediator _mediator;
        public MessageController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost]
        public async Task<IActionResult> SendChatMessageAsync([FromBody] MessageDTO dto)
        {
            try
            {
                var userId = CurrentUserId;
                if (dto.ChatID == Guid.Empty) return BadRequest();

                var result = await _mediator.Send(new SendChatMessageCommand(dto));
                return result ? Ok() : BadRequest();
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
            var messages = await _mediator.Send(new GetChatMessageHistoryQuery(userId, chatId));
            return Ok(messages);
        }
        [HttpPatch("chat/{chatId}/read/{messageId}")]
        public async Task<IActionResult> MarkMessageAsReadAsync([FromRoute] Guid chatId, [FromRoute] Guid messageId)
        {
            var userId = CurrentUserId;
            if (chatId == Guid.Empty) return BadRequest();
            var result = await _mediator.Send(new MarkMessageAsReadCommand(userId,chatId,messageId));
            return result ? Ok() : BadRequest();
        }
        [HttpPatch("chat/{chatId}/read-all")]
        public async Task<IActionResult> MarkAllMessagesAsReadAsync([FromRoute] Guid chatId,CancellationToken ct)
        {
            var userId = CurrentUserId;
            if (chatId == Guid.Empty) return BadRequest();
            var result = await _mediator.Send(new MarkAllMessagesAsReadCommand(userId, chatId));
            return result ? Ok() : BadRequest();
        }
    }
}

