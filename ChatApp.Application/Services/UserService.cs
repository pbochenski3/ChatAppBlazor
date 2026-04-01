using ChatApp.Application.DTO;
using ChatApp.Application.Interfaces.Repository;
using ChatApp.Application.Interfaces.Service;
using ChatApp.Domain.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Services
{
    public class UserService : IUserService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly IUserRepository _userRepo;

        public UserService(IUserRepository userRepo, IOptions<JwtSettings> jwtSettings)
        {
            _userRepo = userRepo;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task RegisterUserAsync(UserDTO userDto)
        {
            if (string.IsNullOrWhiteSpace(userDto.Username) || userDto.Username.Length < 5)
            {
                throw new Exception("Username must be at least 5 characters.");
            }
            if (string.IsNullOrWhiteSpace(userDto.Password) || userDto.Password.Length < 8)
            {
                throw new Exception("Password must be at least 8 characters.");
            }
            if (userDto.Username.Length > 20)
            {
                throw new Exception("Username must be less than 20 characters.");
            }
            if (userDto.Password.Length > 128)
            {
                throw new Exception("Password must be less than 128 characters.");
            }

            var existingUser = await _userRepo.GetByUsernameAsync(userDto.Username);
            if (existingUser != null)
            {
                throw new Exception("Username already exists.");
            }

            var user = new User
            {
                Username = userDto.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(userDto.Password),
                AvatarUrl = $"https://localhost:7255/cdn/avatars/default-avatar.png"
            };
            await _userRepo.RegisterAsync(user);
        }
        public async Task<UserDTO?> LoginUserAsync(UserDTO userDto)
        {
            if (string.IsNullOrWhiteSpace(userDto.Username) || string.IsNullOrWhiteSpace(userDto.Password))
            {
                throw new Exception("Username or password cannot be empty.");
            }

            var user = await _userRepo.GetByUsernameAsync(userDto.Username);
            if (user != null && BCrypt.Net.BCrypt.Verify(userDto.Password, user.Password))
            {
                await _userRepo.SetUserStatusAsync(user.UserID, true);
                var token = GenerateToken(user);
                return new UserDTO
                {
                    Username = user.Username,
                    UserID = user.UserID,
                    IsOnline = true,
                    Token = token,
                    AvatarUrl = user.AvatarUrl
                };
            }

            throw new Exception("Invalid username or password.");
        }
        public async Task<List<UserDTO>> GetUsersToInviteAsync(Guid currentUserId, string query)
        {
            var users = await _userRepo.GetAllUsersToInviteAsync(currentUserId, query);
            return users.Select(u => new UserDTO
            {
                UserID = u.UserID,
                Username = u.Username,
                AvatarUrl = u.AvatarUrl
            }).ToList();
        }
        public async Task<HashSet<UserDTO>> GetUsersByIdsAsync(HashSet<Guid> userIds)
        {
            var users = await _userRepo.GetUsersByIdsAsync(userIds);
            return users.Select(u => new UserDTO
            {
                UserID = u.UserID,
                Username = u.Username,
                AvatarUrl = u.AvatarUrl,
                IsOnline = u.IsOnline
            }).ToHashSet();
        }
        public async Task<UserDTO?> GetUserByIdAsync(Guid userId)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null) return null;

            return new UserDTO
            {
                UserID = user.UserID,
                Username = user.Username,
                AvatarUrl = user.AvatarUrl,
                IsOnline = user.IsOnline
            };
        }
        public async Task UpdateUserAvatarAsync(Guid userId,string avatarUrl)
        {
            if (string.IsNullOrWhiteSpace(avatarUrl) || !Uri.IsWellFormedUriString(avatarUrl, UriKind.RelativeOrAbsolute))
            {
                throw new Exception("Invalid avatar URL.");
            }
            await _userRepo.UpdateAvatarAsync(userId, avatarUrl);
        }
        public async Task<string> GetAvatarUrlAsync(Guid userId)
        {
            return await _userRepo.GetAvatarUrlAsync(userId);
        }

        private string GenerateToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };
            var tokenDescriptor = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}
