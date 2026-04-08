using ChatApp.Application.DTO;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Interfaces.Chats;
using ChatApp.Application.Interfaces.Service;
using ChatApp.Application.Services;
using ChatApp.Domain.Enums;
using ChatApp.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChatApp.ChatHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IFileService _fileService;
        private readonly IContactService _contactService;
        private readonly IUserService _userService;

        public UserController(IFileService fileService,IHubContext<ChatHub> hubContext,IContactService contactService,IUserService userService)
        {
            _hubContext = hubContext;
            _fileService = fileService;
            _contactService = contactService;
            _userService = userService;
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
                string avatarUrl = await _fileService.SaveAvatar(stream, extension,UploadType.UserAvatar);
                await _userService.UpdateUserAvatarAsync(userGuid, avatarUrl);
                var contacts = await _contactService.GetUserContactsAsync(userGuid);
                var contactsToNofitify = contacts.Select(c => c.ContactUserID.ToString()).ToList();
                var allRecipients = contactsToNofitify.Append(userGuid.ToString()).ToList();
                await _hubContext.Clients.Users(allRecipients).SendAsync("ContactAvatarReload", avatarUrl, userGuid);
                return Ok();
                    }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
