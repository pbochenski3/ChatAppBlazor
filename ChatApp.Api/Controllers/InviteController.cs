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
            _logger.LogInformation("SendContactInvite: sender={SenderId}, receiver={ReceiverId}", senderId, receiverId);
            await Task.WhenAll(
                _hubContext.Clients.User(senderId.ToString()).SendAsync("ReceiveStatus", "Zaproszenie wysłane!"),
                _hubContext.Clients.User(receiverId.ToString()).SendAsync("InviteReload", true),
                _hubContext.Clients.User(receiverId.ToString()).SendAsync("ReceiveStatus", "Otrzymałeś nowe zaproszenie!")
            );
            _logger.LogDebug("Invite notifications sent for invite from {SenderId} to {ReceiverId}", senderId, receiverId);
            return Ok();
        }
        [HttpPost("action")]
        public async Task<IActionResult> HandleInviteActionAsync([FromBody] InviteActionRequest request, CancellationToken ct)
        {
            try
            {
                var actionUserId = CurrentUserId;
                await _inviteService.UpdateInviteStatusAsync(request.InviteId, request.Response);
                await _hubContext.Clients.User(actionUserId.ToString()).SendAsync("InviteReload", true);
                var invite = await _inviteService.ProcessInviteActionAsync(request.InviteId, request.Response, ct);

                if (invite == null)
                {
                    return NotFound("Invite not found");
                }

                var receiverId = invite.senderId;
                await Task.WhenAll(
                    _hubContext.Clients.User(receiverId.ToString()).SendAsync("ReceiveStatus", $"Twoje zaproszenie zostało {(request.Response == InviteStatus.Accepted ? "zaakceptowane" : "odrzucone")}"),
                    _hubContext.Clients.User(actionUserId.ToString()).SendAsync("ReceiveStatus", $"{(request.Response == InviteStatus.Accepted ? "Zaakceptowałeś" : "Odrzuciłeś")} zaproszenie!")
                );
                await Task.WhenAll(
                    _hubContext.Clients.User(receiverId.ToString()).SendAsync("SideBarReload", true),
                    _hubContext.Clients.User(actionUserId.ToString()).SendAsync("SideBarReload", true)
                );

                if (invite.chatId == request.chatId)
                {
                    await Task.WhenAll(
                        _hubContext.Clients.User(actionUserId.ToString()).SendAsync("ChatReload", invite.chatId, true),
                        _hubContext.Clients.User(receiverId.ToString()).SendAsync("ChatReload", invite.chatId, true)
                    );
                }

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
