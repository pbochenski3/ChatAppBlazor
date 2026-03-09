
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace ChatApp.Infrastructure.Providers
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            return connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        }
    }
}
