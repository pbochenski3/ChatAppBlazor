using ChatApp.Application.DTO;
using ChatApp.Application.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.ChatHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IUserService _userService;
        public AuthController(IUserService userService, IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
            _userService = userService;
        }
        [HttpPost("login")]
        public async Task<IActionResult> LoginUserAsync([FromBody] UserDTO loginDto)
        {
            try
            {
                var user = await _userService.LoginUserAsync(loginDto);
                if (user == null)
                    return Unauthorized("Błędny login lub hasło");
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUserAsync([FromBody] UserDTO dto)
        {
            try
            {
                await _userService.RegisterUserAsync(dto);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
