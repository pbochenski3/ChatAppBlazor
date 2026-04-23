using ChatApp.Application.DTO.Requests;
using ChatApp.Application.Feature.Invite.GetUsersInvites;
using ChatApp.Application.Feature.Invite.HandleInviteAction;
using ChatApp.Application.Feature.Invite.SendContactInvite;
using ChatApp.Application.Interfaces.Chats;
using ChatApp.Application.Interfaces.Service;
using ChatApp.Application.Notifications.Invite;
using ChatApp.Domain.Enums;
using ChatApp.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace ChatApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InviteController : AppControllerBase
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IInviteService _inviteService;
        private readonly ILogger<InviteController> _logger;
        private readonly IMediator _mediator;
        public InviteController(IHubContext<ChatHub> hubContext, IInviteService inviteService, ILogger<InviteController> logger,IMediator mediator)
        {
            _hubContext = hubContext;
            _inviteService = inviteService;
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
