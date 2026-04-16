using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChatApp.Api.Controllers
{
    [ApiController]
    [Authorize]
    public abstract class AppControllerBase : ControllerBase
    {
        protected Guid CurrentUserId
        {
            get
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (Guid.TryParse(userId, out var guid)) return guid;
                throw new UnauthorizedAccessException("Brak poprawnego ID użytkownika w tokenie.");
            }
        }
    }
}
