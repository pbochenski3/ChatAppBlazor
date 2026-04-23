using ChatApp.Application.DTO.Requests;
using ChatApp.Application.Feature.Invite.GetUsersInvites;
using ChatApp.Application.Feature.Invite.HandleInviteAction;
using ChatApp.Application.Feature.Invite.SendContactInvite;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InviteController : AppControllerBase
    {
        private readonly ILogger<InviteController> _logger;
        private readonly IMediator _mediator;
        public InviteController(ILogger<InviteController> logger, IMediator mediator)
        {
            _mediator = mediator;
            _logger = logger;
        }
        [HttpGet]
        public async Task<IActionResult> GetUserInvitesAsync()
        {
            var userId = CurrentUserId;
            var invites = await _mediator.Send(new GetUserInvitesQuery(userId));
            return Ok(invites);
        }
        [HttpPost]
        public async Task<IActionResult> SendContactInviteAsync([FromBody] Guid receiverId)
        {
            var senderId = CurrentUserId;
            await _mediator.Send(new SendContactInviteCommand(senderId, receiverId));
            return Ok();
        }
        [HttpPost("action")]
        public async Task<IActionResult> HandleInviteActionAsync([FromBody] InviteActionRequest request, CancellationToken ct)
        {
            var actionUserId = CurrentUserId;
            await _mediator.Send(new HandleInviteActionCommand(request));
            return Ok();
        }
    }
}
