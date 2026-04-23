using ChatApp.Application.Feature.Sidebar.GetSidebarItems;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Api.Controllers
{
    [ApiController]
    [Route("api/sidebar")]
    public class SidebarController : AppControllerBase
    {
        private readonly ILogger<SidebarController> _logger;
        private readonly IMediator _mediator;
        public SidebarController(ILogger<SidebarController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }
        [HttpGet]
        public async Task<IActionResult> GetSidebarItemsAsync()
        {
            var userId = CurrentUserId;
            try
            {
                var items = await _mediator.Send(new GetSidebarItemsQuery(userId));
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting sidebar items for user {UserId}", userId);
                return StatusCode(500, "An error occurred while retrieving sidebar items.");
            }
        }
    }
}
