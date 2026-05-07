using ChatApp.Application.DTO;
using ChatApp.Application.Feature.Messages.EditMessage;
using ChatApp.Application.Feature.Messages.GetChatMessageHistory;
using ChatApp.Application.Feature.Messages.MarkAllMessagesAsRead;
using ChatApp.Application.Feature.Messages.MarkAsRead;
using ChatApp.Application.Feature.Messages.SendChatMessage;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

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
        [EnableRateLimiting("chat-send-policy")]
        [HttpPost]
        public async Task<IActionResult> SendChatMessageAsync([FromBody] MessageDTO dto)
        {
            var userId = CurrentUserId;
            if (dto.ChatID == Guid.Empty) return BadRequest();
            var result = await _mediator.Send(new SendChatMessageCommand(dto));
            return result ? Ok() : BadRequest();
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
            var result = await _mediator.Send(new MarkMessageAsReadCommand(userId, chatId, messageId));
            return result ? Ok() : BadRequest();
        }
        [HttpPatch("chat/{chatId}/read-all")]
        public async Task<IActionResult> MarkAllMessagesAsReadAsync([FromRoute] Guid chatId, CancellationToken ct)
        {
            var userId = CurrentUserId;
            if (chatId == Guid.Empty) return BadRequest();
            var result = await _mediator.Send(new MarkAllMessagesAsReadCommand(userId, chatId));
            return result ? Ok() : BadRequest();
        }
        [HttpPatch("{chatId}/{messageId}/edit")]
        public async Task<IActionResult> EditMessageAsync([FromRoute] Guid chatId, [FromRoute] Guid messageId, [FromBody] string content, CancellationToken ct)
        {
            var userId = CurrentUserId;
            if (chatId == Guid.Empty) return BadRequest();
            var result = await _mediator.Send(new EditMessageCommand(chatId, messageId, content, userId));
            return result ? Ok() : BadRequest();
        }
    }
}

