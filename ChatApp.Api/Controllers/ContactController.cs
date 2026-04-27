using ChatApp.Application.Feature.Contact.DeleteContact;
using ChatApp.Application.Feature.Contact.GetUserContacts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactController : AppControllerBase
    {
        private readonly IMediator _mediator;
        public ContactController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet("{chatId}")]
        public async Task<IActionResult> GetUserContactsAsync([FromRoute] Guid chatId, CancellationToken ct)
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
