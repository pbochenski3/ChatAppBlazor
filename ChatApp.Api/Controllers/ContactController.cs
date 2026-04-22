using ChatApp.Application.Interfaces.Chats;
using ChatApp.Application.Interfaces.Service;
using ChatApp.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactController : AppControllerBase
    {
        private readonly IContactService _contactService;
        private readonly IPrivateChatService _privateChatService;
        private readonly IChatService _chatService;
        public ContactController(IContactService contactService, IPrivateChatService privateChatService, IChatService chatService)
        {
            _contactService = contactService;
            _privateChatService = privateChatService;
            _chatService = chatService;
        }
        [HttpGet("{chatId}")]
        public async Task<IActionResult> GetUserContactsAsync([FromRoute] Guid chatId,CancellationToken ct)
        {
            var contacts = await _contactService.GetChatContactAsync(CurrentUserId,chatId);
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
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
