using ChatApp.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.ChatHub.Controllers
{
    [ApiController]
    [Route("api/sidebar")]
    public class SidebarController : AppControllerBase
    {
        private readonly ILogger<SidebarController> _logger;
        private readonly ISidebarService _sidebarService;
        public SidebarController(ILogger<SidebarController> logger, ISidebarService sidebarService)
        {
            _logger = logger;
            _sidebarService = sidebarService;
        }
        [HttpGet]
        public async Task<IActionResult> GetSidebarItemsAsync()
        {
            var userId = CurrentUserId;
            try
            {
                var items = await _sidebarService.GetSidebarItemsAsync(userId);
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
