using ChatApp.Application.DTO.Requests;
using ChatApp.Application.Feature.Chat.UpdateChatName;
using ChatApp.Application.Feature.Chat.UpdateUserAlias;
using ChatApp.Application.Feature.User.GetUsersToInvite;
using ChatApp.Application.Feature.User.UpdateUserAvatar;
using ChatApp.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : AppControllerBase
    {
        private readonly IFileService _fileService;
        private readonly IMediator _mediator;

        public UserController(IFileService fileService, IMediator mediator)
        {
            _fileService = fileService;
            _mediator = mediator;
        }


        [HttpGet("to-invite")]
        public async Task<IActionResult> GetUsersToInviteAsync([FromQuery] string query)
        {
            var userId = CurrentUserId;
            var usersToInvite = await _mediator.Send(new GetUsersToInviteQuery(userId, query));
            return Ok(usersToInvite);
        }
        [HttpPost("avatar")]
        public async Task<IActionResult> UploadAvatarAsync(IFormFile file)
        {
            var userId = CurrentUserId;
            try
            {
                var result = await _mediator.Send(new UpdateUserAvatarCommand(file, userId));
                return result ? Ok() : BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    
    }
}
