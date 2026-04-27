using ChatApp.Application.DTO;
using ChatApp.Application.Feature.Auth.LoginUser;
using ChatApp.Application.Feature.Auth.RefreshToken;
using ChatApp.Application.Feature.Auth.RegisterUser;
using ChatApp.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IMediator _mediator;
        public AuthController(IHubContext<ChatHub> hubContext, IMediator mediator)
        {
            _hubContext = hubContext;
            _mediator = mediator;
        }
        [HttpPost("login")]
        public async Task<IActionResult> LoginUserAsync([FromBody] UserDTO loginDto)
        {
            try
            {
                string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                var response = await _mediator.Send(new LoginUserCommand(loginDto, ipAddress));
                if (response == null) return Unauthorized("Błędny login lub hasło");
                SetRefreshTokenCookie(response.RefreshToken);
                return Ok(new
                {
                    token = response.AccessToken,
                    user = response.User
                });
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
                var result = await _mediator.Send(new RegisterUserCommand(dto));
                return result ? Ok() : BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken)) return Unauthorized();
            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var result = await _mediator.Send(new RefreshTokenCommand(refreshToken, ipAddress));
            if (result == null)
                return Unauthorized("Sesja wygasła lub token jest nieprawidłowy");

            SetRefreshTokenCookie(result.RefreshToken);
            return Ok(new
            {
                token = result.AccessToken,
                user = result.User
            });
        }
        private void SetRefreshTokenCookie(string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }
    }
}
