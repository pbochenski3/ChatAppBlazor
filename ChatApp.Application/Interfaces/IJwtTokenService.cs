using ChatApp.Domain.Entities;

namespace ChatApp.Application.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateAccessToken(User user);
    }
}
