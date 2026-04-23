using ChatApp.Application.DTO.Chats;
using ChatApp.Application.DTO.Requests;
using ChatApp.Application.Feature.Chat.CheckGroupChatExists;
using ChatApp.Application.Feature.Chat.DeleteChat;
using ChatApp.Application.Feature.Chat.GetChatDetails;
using ChatApp.Application.Feature.Chat.GetChatUsers;
using ChatApp.Application.Feature.Chat.UpdateChatName;
using ChatApp.Application.Feature.File.SaveChatImage;
using ChatApp.Application.Feature.File.SaveGroupAvatar;
using ChatApp.Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class ChatController : AppControllerBase
    {
        private readonly IMediator _mediator;

        public ChatController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{chatId}/details")]
        public async Task<ActionResult<UserChatDTO>> GetChatDetailsAsync([FromRoute] Guid chatId, CancellationToken ct)
        {
            var userId = CurrentUserId;
            if (chatId == Guid.Empty) return BadRequest();
            var chatDetails = await _mediator.Send(new GetChatDetailsQuery(chatId, userId));
            if (chatDetails == null) return NotFound();
            return Ok(chatDetails);
        }
        [HttpPost("chatImage")]
        public async Task<IActionResult> SaveChatImageAsync(IFormFile file, [FromQuery] Guid chatId)
        {
            var userId = CurrentUserId;
            if (chatId == Guid.Empty) return BadRequest();

            string imageUrl = await _mediator.Send(new SaveChatImageCommand(file, chatId, userId));
            var response = new FileResponse { Url = imageUrl };
            return Ok(response);
        }
        [HttpPost("groupAvatar")]
        public async Task<IActionResult> SaveGroupAvatar(IFormFile file, [FromQuery] Guid chatId)
        {
            var userId = CurrentUserId;
            if (chatId == Guid.Empty) return BadRequest();
            await _mediator.Send(new SaveGroupAvatarCommand(file, chatId, userId));
            return Ok();
        }
        [HttpDelete("{chatId}")]
        public async Task<IActionResult> DeleteChatAsync([FromRoute] Guid chatId)
        {
            var userId = CurrentUserId;
            if (chatId == Guid.Empty) return BadRequest();
            try
            {
                bool result = await _mediator.Send(new DeleteChatCommand(chatId, userId));
                return result ? Ok() : BadRequest();
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

            bool result = await _mediator.Send(new UpdateChatNameCommand(chatId, userId, request));
            return result ? Ok() : BadRequest();
        }
        [HttpGet("{chatId}/existing")]
        public async Task<IActionResult> CheckGroupChatExistsAsync([FromRoute] Guid chatId, CancellationToken ct)
        {
            var userId = CurrentUserId;
            if (chatId == Guid.Empty) return BadRequest();
            var exists = await _mediator.Send(new CheckGroupChatExistsQuery(chatId, userId));
            return Ok(exists);
        }
        [HttpGet("{chatId}/usersId")]
        public async Task<IActionResult> GetChatUsersAsync([FromRoute] Guid chatId, CancellationToken ct)
        {
            var ids = await _mediator.Send(new GetChatUsersIdsQuery(chatId));
            return Ok(ids);

        }
    }
}
