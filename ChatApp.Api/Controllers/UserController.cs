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

namespace ChatApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : AppControllerBase
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IFileService _fileService;
        private readonly IContactService _contactService;
        private readonly IUserService _userService;

        public UserController(IFileService fileService, IHubContext<ChatHub> hubContext, IContactService contactService, IUserService userService)
        {
            _hubContext = hubContext;
            _fileService = fileService;
            _contactService = contactService;
            _userService = userService;
        }
        [HttpGet("to-invite")]
        public async Task<IActionResult> GetUsersToInviteAsync([FromQuery] string query)
        {
            var userId = CurrentUserId;
            var usersToInvite = await _userService.GetUsersToInviteAsync(userId, query);
            return Ok(usersToInvite);
        }
        [HttpPost("avatar")]
        public async Task<IActionResult> UploadAvatarAsync(IFormFile file)
        {
            var userId = CurrentUserId;
            try
            {
                var extension = Path.GetExtension(file.FileName).ToLower();
                using var stream = file.OpenReadStream();
                string avatarUrl = await _fileService.SaveAvatar(stream, extension,UploadType.UserAvatar);
                await _userService.UpdateUserAvatarAsync(userId, avatarUrl);
                var contacts = await _contactService.GetUserContactsAsync(userId);
                var contactsToNofitify = contacts.Select(c => c.ContactUserID.ToString()).ToList();
                var allRecipients = contactsToNofitify.Append(userId.ToString()).ToList();
                await _hubContext.Clients.Users(allRecipients).SendAsync("ContactAvatarReload", avatarUrl, userId);
                return Ok();
                    }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
