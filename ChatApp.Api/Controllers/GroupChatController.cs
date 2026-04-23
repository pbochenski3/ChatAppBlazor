using ChatApp.Application.DTO;
using ChatApp.Application.Feature.GroupChat.AddUsersToGroupChat;
using ChatApp.Application.Feature.GroupChat.CreateGroupChat;
using ChatApp.Application.Feature.GroupChat.GetChatUsers;
using ChatApp.Application.Feature.GroupChat.LeaveGroupChatAsync;
using ChatApp.Application.Interfaces.Chats;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GroupChatController : AppControllerBase
    {
        private readonly IMediator _mediator;


        public GroupChatController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("{chatId}/add-users")]
        public async Task<IActionResult> AddUsersToGroupChatAsync([FromBody] HashSet<Guid> usersToAdd, [FromRoute] Guid chatId, CancellationToken ct)
        {

            var userId = CurrentUserId;
            try
            {
                await _mediator.Send(new AddUsersToGroupChatCommand(chatId, usersToAdd, userId));
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("from-private-chat/{chatId}")]

        public async Task<IActionResult> CreateGroupChatAsync([FromBody] HashSet<Guid> usersToAdd, [FromRoute] Guid chatId, CancellationToken ct)
        {

            var userId = CurrentUserId;
            try
            {
                await _mediator.Send(new CreateGroupChatCommand(chatId, usersToAdd, userId));
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{chatId}/{username}")]
        public async Task<IActionResult> LeaveGroupChatAsync(Guid chatId, string username)
        {
            var userId = CurrentUserId;
            try
            {
                await _mediator.Send(new LeaveGroupChatCommand(chatId, userId, username));

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("{chatId}/users")]
        public async Task<IActionResult> GetChatUsersAsync([FromRoute] Guid chatId, CancellationToken ct)
        {
            try
            {
                HashSet<UserDTO> users = await _mediator.Send(new GetChatUsersQuery(chatId));

                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
