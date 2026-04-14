using ChatApp.Application.Interfaces.Chats;
using ChatApp.Application.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.ChatHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactController : AppControllerBase
    {
        private readonly IContactService _contactService;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IPrivateChatService _privateChatService;
        public ContactController(IContactService contactService, IHubContext<ChatHub> hubContext, IPrivateChatService privateChatService)
        {
            _contactService = contactService;
            _hubContext = hubContext;
            _privateChatService = privateChatService;
        }
        [HttpGet]
        public async Task<IActionResult> GetUserContactsAsync(CancellationToken ct)
        {
            var contacts = await _contactService.GetUserContactsAsync(CurrentUserId);
            return Ok(contacts);
        }
        [HttpDelete("delete/by-chat/{privateChatId}")]
        public async Task<IActionResult> DeleteContactAsync([FromRoute] Guid privateChatId, CancellationToken ct)
        {
            var userId = CurrentUserId;
            if (privateChatId == Guid.Empty)
            {
                return BadRequest();
            }
            var contactId = await _privateChatService.GetReceiverUserIdAsync(privateChatId, userId, ct);
            if (contactId == Guid.Empty)
            {
                return BadRequest();
            }
            try
            {
                await _contactService.RemoveContactAsync(contactId, userId, privateChatId);
                var recipients = new List<string> { userId.ToString(), contactId.ToString() };
                await _hubContext.Clients.Users(recipients).SendAsync("ChatReload", privateChatId, true);
                await Task.WhenAll(
                    _hubContext.Clients.Users(userId.ToString()).SendAsync("ReceiveStatus", "Kontakt został usunięty!"),
                    _hubContext.Clients.Users(contactId.ToString()).SendAsync("ReceiveStatus", "Ktoś usunął cie z kontaktów!")
                );

                await _hubContext.Clients.Users(recipients).SendAsync("ContactInviteReload");
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
