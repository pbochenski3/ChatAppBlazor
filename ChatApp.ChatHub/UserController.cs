using ChatApp.Application.DTO;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Interfaces.Service;
using ChatApp.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChatApp.ChatHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IFileService _fileService;

        public UserController(IUserService userService, IFileService fileService)
        {
            _userService = userService;
            _fileService = fileService;
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
        [Authorize]
        [HttpPost("updateAvatar")]
        public async Task<IActionResult> UploadAvatarAsync(IFormFile file)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
            {
                return Unauthorized();
            }
            try
            {
                var extension = Path.GetExtension(file.FileName).ToLower();
                using var stream = file.OpenReadStream();
                string avatarUrl = await _fileService.SaveUserAvatarAsync(stream, extension);
                await _userService.UpdateUserAvatarAsync(userGuid, avatarUrl);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
