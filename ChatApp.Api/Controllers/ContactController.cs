using ChatApp.Application.Feature.Contact.DeleteContact;
using ChatApp.Application.Feature.Contact.GetUserContacts;
using ChatApp.Application.Interfaces.Chats;
using ChatApp.Application.Interfaces.Service;
using ChatApp.Domain.Models;
using MediatR;
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
        private readonly IMediator _mediator;
        public ContactController(IContactService contactService, IPrivateChatService privateChatService, IChatService chatService,IMediator mediator)
        {
            _contactService = contactService;
            _privateChatService = privateChatService;
            _chatService = chatService;
            _mediator = mediator;
        }
        [HttpGet("{chatId}")]
        public async Task<IActionResult> GetUserContactsAsync([FromRoute] Guid chatId,CancellationToken ct)
        {
            var contacts = await _mediator.Send(new GetUserContactsQuery(CurrentUserId, chatId));
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
            var result = await _mediator.Send(new DeleteContactCommand(privateChatId, userId));
            return result ? Ok() : BadRequest();
        }
    }
}
