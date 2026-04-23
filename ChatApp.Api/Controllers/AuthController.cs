using ChatApp.Application.DTO;
using ChatApp.Application.Feature.Auth.LoginUser;
using ChatApp.Application.Feature.Auth.RegisterUser;
using ChatApp.Application.Interfaces.Service;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IUserService _userService;
        private readonly IMediator _mediator;
        public AuthController(IUserService userService, IHubContext<ChatHub> hubContext, IMediator mediator)
        {
            _hubContext = hubContext;
            _userService = userService;
            _mediator = mediator;
        }
        [HttpPost("login")]
        public async Task<IActionResult> LoginUserAsync([FromBody] UserDTO loginDto)
        {
            try
            {
                var user = await _mediator.Send(new LoginUserCommand(loginDto));
                if (user == null) return Unauthorized("Błędny login lub hasło");
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
                var  result = await _mediator.Send(new RegisterUserCommand(dto));
                return result ? Ok() : BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
