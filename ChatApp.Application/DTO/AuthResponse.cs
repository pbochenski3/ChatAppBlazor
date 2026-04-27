using ChatApp.Domain.Models;

namespace ChatApp.Application.DTO
{
    public class AuthResponse
    {
        public UserDTO? User { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }

        public static AuthResponse CreateResponse(User user, string accessToken, string refreshToken)
        {
            return new AuthResponse
            {
                User = new UserDTO
                {
                    Username = user.Username,
                    UserID = user.UserID,
                    IsOnline = true,
                    AvatarUrl = user.AvatarUrl
                },
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
    }
}