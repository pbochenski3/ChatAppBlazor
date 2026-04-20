using ChatApp.Application.DTO.Requests;
using ChatApp.Application.Interfaces.Chats;
using ChatApp.Application.Interfaces.Service;
using ChatApp.Domain.Enums;
using ChatApp.Domain.Models;
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
        public InviteController(IHubContext<ChatHub> hubContext, IInviteService inviteService, ILogger<InviteController> logger)
        {
            _hubContext = hubContext;
            _inviteService = inviteService;
            _logger = logger;
        }
        [HttpGet]
        public async Task<IActionResult> GetUserInvitesAsync()
        {
            var userId = CurrentUserId;
            var invites = await _inviteService.GetUserInvitesAsync(userId);
            return Ok(invites);
        }
        [HttpPost]
        public async Task<IActionResult> SendContactInviteAsync([FromBody] Guid receiverId)
        {
            var senderId = CurrentUserId;
            await _inviteService.SendInviteAsync(senderId, receiverId);
            return Ok();
        }
        [HttpPost("action")]
        public async Task<IActionResult> HandleInviteActionAsync([FromBody] InviteActionRequest request, CancellationToken ct)
        {
            try
            {
                var actionUserId = CurrentUserId;
                await _inviteService.ProcessInviteActionAsync(request, ct);
                return Ok();
            }
            catch (KeyNotFoundException knf)
            {
                return NotFound(knf.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
