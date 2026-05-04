using ChatApp.Application.DTO.Result;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Interfaces.Repository;
using MediatR;

namespace ChatApp.Application.Feature.Auth.RegisterUser
{
    public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, RegisterResult>
    {
        private readonly IUserRepository _userRepo;
        public RegisterUserHandler(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<RegisterResult> Handle(RegisterUserCommand r, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(r.RegisterData.Username) || r.RegisterData.Username.Length < 5)
            {
                return new RegisterResult(false, "Nazwa użytkownika musi składać się z co najmniej 5 znaków.");
            }
            if (string.IsNullOrWhiteSpace(r.RegisterData.Password) || r.RegisterData.Password.Length < 8)
            {
                return new RegisterResult(false, "Hasło musi składać się z co najmniej 8 znaków.");
            }
            if (r.RegisterData.Username.Length > 20)
            {
                return new RegisterResult(false, "Nazwa użytkownika nie może przekraczać 20 znaków.");
            }
            if (r.RegisterData.Password.Length > 128)
            {
                return new RegisterResult(false, "Hasło nie może przekraczać 128 znaków.");
            }

            var existingUser = await _userRepo.GetByUsernameAsync(r.RegisterData.Username);
            if (existingUser != null)
            {
                return new RegisterResult(false, "Podana nazwa użytkownika jest już zajęta.");
            }

            var user = new User
            {
                Username = r.RegisterData.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(r.RegisterData.Password),
                AvatarUrl = $"https://localhost:7256/cdn/avatars/default-avatar.png"
            };
            try
            {
                await _userRepo.RegisterAsync(user);
            return new RegisterResult(true, "Rejestracja przebiegła pomyślnie!");
            }
            catch
            {
            return new RegisterResult(true, "Rejestracja się nie udała!");
            }

        }
    }

}
