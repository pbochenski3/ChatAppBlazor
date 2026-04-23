using ChatApp.Application.Interfaces.Repository;
using ChatApp.Domain.Models;
using MediatR;

namespace ChatApp.Application.Feature.Auth.RegisterUser
{
    public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, bool>
    {
        private readonly IUserRepository _userRepo;
        public RegisterUserHandler(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<bool> Handle(RegisterUserCommand r, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(r.RegisterData.Username) || r.RegisterData.Username.Length < 5)
            {
                throw new Exception("Username must be at least 5 characters.");
            }
            if (string.IsNullOrWhiteSpace(r.RegisterData.Password) || r.RegisterData.Password.Length < 8)
            {
                throw new Exception("Password must be at least 8 characters.");
            }
            if (r.RegisterData.Username.Length > 20)
            {
                throw new Exception("Username must be less than 20 characters.");
            }
            if (r.RegisterData.Password.Length > 128)
            {
                throw new Exception("Password must be less than 128 characters.");
            }

            var existingUser = await _userRepo.GetByUsernameAsync(r.RegisterData.Username);
            if (existingUser != null)
            {
                throw new Exception("Username already exists.");
            }

            var user = new User
            {
                Username = r.RegisterData.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(r.RegisterData.Password),
                AvatarUrl = $"https://localhost:7255/cdn/avatars/default-avatar.png"
            };
            await _userRepo.RegisterAsync(user);
            return true;
        }
    }

}
