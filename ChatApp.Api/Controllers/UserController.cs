using ChatApp.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : AppControllerBase
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IFileService _fileService;

        public UserController(IFileService fileService, IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
            _fileService = fileService;
        }
        //[HttpGet("to-invite")]
        //public async Task<IActionResult> GetUsersToInviteAsync([FromQuery] string query)
        //{
        //    var userId = CurrentUserId;
        //    var usersToInvite = await _userService.GetUsersToInviteAsync(userId, query);
        //    return Ok(usersToInvite);
        //}
        //[HttpPost("avatar")]
        //public async Task<IActionResult> UploadAvatarAsync(IFormFile file)
        //{
        //    var userId = CurrentUserId;
        //    try
        //    {
        //        await _userService.UpdateUserAvatarAsync(userId, file);
        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}
    }
}
