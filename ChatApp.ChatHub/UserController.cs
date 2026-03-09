using ChatApp.Application.DTO;
using ChatApp.Application.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserDTO loginDto)
    {
        try
        {

            var user = await _userService.Login(loginDto);

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
        public async Task<IActionResult> Register([FromBody] UserDTO dto)
        {
            try
            {
                await _userService.Register(dto);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

