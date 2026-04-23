using ChatApp.Application.DTO;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Interfaces.Repository;
using MediatR;

namespace ChatApp.Application.Feature.Auth.LoginUser
{
    public class LoginUserHandler : IRequestHandler<LoginUserCommand, UserDTO>
    {
        private readonly IUserRepository _userRepo;
        private readonly IJwtTokenService _jwt;
        public LoginUserHandler(IUserRepository userRepo, IJwtTokenService jwt)
        {
            _userRepo = userRepo;
            _jwt = jwt;
        }
        public async Task<UserDTO> Handle(LoginUserCommand r, CancellationToken cancellationToken)
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
                return UserDTO.CreateUserDto(user, token);
            }
            throw new Exception("Błędny login lub hasło");
        }
    }
}
