using ChatApp.Application.DTO;
using ChatApp.Application.Interfaces;
using ChatApp.Domain.Interfaces.Repository;
using ChatApp.Domain.Models;
using MediatR;

namespace ChatApp.Application.Feature.Auth.LoginUser
{
    public class LoginUserHandler : IRequestHandler<LoginUserCommand, AuthResponse>
    {
        private readonly IUserRepository _userRepo;
        private readonly IJwtTokenService _jwt;
        public LoginUserHandler(IUserRepository userRepo, IJwtTokenService jwt)
        {
            _userRepo = userRepo;
            _jwt = jwt;
        }

        public async Task<AuthResponse> Handle(LoginUserCommand r, CancellationToken cancellationToken)
        {

            if (string.IsNullOrWhiteSpace(r.LoginData.Username) || string.IsNullOrWhiteSpace(r.LoginData.Password))
            {
                throw new Exception("Username or password cannot be empty.");
            }
            var user = await _userRepo.GetByUsernameAsync(r.LoginData.Username);
            if (user != null && BCrypt.Net.BCrypt.Verify(r.LoginData.Password, user.Password))
            {
                await _userRepo.SetUserStatusAsync(user.UserID, true);
                var token = _jwt.GenerateAccessToken(user);
                var refreshToken = _jwt.GenerateRefreshToken();
                var refreshTokenEntity = new UserRefreshToken
                {
                    UserId = user.UserID,
                    Token = refreshToken,
                    ExpiryDate = DateTime.UtcNow.AddDays(7),
                    CreatedByIp = r.IpAdress 
                };
                await _userRepo.AddRefreshTokenAsync(refreshTokenEntity);
                return AuthResponse.CreateResponse(user, token, refreshToken);
            }
            throw new Exception("Błędny login lub hasło");
        }
    }
}
