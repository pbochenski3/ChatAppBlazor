using ChatApp.Domain.Models;

namespace ChatApp.Application.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateAccessToken(User user);
    }
}
