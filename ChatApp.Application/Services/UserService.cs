using ChatApp.Application.DTO;
using ChatApp.Application.Interfaces.Repository;
using ChatApp.Application.Interfaces.Service;
using ChatApp.Domain.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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
        public async Task Register(UserDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username) || dto.Username.Length < 5)
            {
                throw new Exception("Username must be at least 5 characters.");
            }
            else if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 8)
            {
                throw new Exception("Password must be at least 8 characters.");
            }
            else if (string.IsNullOrWhiteSpace(dto.Username) || dto.Username.Length > 20)
            {
                throw new Exception("Username must be less than 20 characters.");
            }
            else if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length > 128)
            {
                throw new Exception("Password must be less than 128 characters.");
            }
            var check = await _userRepo.GetByUsernameAsync(dto.Username);
            if (check == null)
            {
                var user = new User
                {
                    Username = dto.Username,
                    Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                    AvatarUrl = $"https://api.dicebear.com/7.x/avataaars/svg?seed={dto.Username}"
                };
                await _userRepo.RegisterAsync(user);
                await _userRepo.SaveChangesToDbAsync();
            }
            else
            {
                throw new Exception("Username already exists.");
            }

        }
        public async Task<UserDTO> Login(UserDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
            {
                throw new Exception("Username or password cannot be empty.");
            }
            var user = await _userRepo.GetByUsernameAsync(dto.Username);
            if (user is not null && BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
            {
                await _userRepo.SetStatus(user.UserID, true);
                var token = GenerateToken(user);
                return new UserDTO() { Username = user.Username, UserID = user.UserID, IsOnline = true, Token = token };
            }
            else
            {
                throw new Exception("Invalid username or password.");
            }
        }
        public async Task<List<UserDTO>> GetAllUsersToInvite(Guid currentUserId, string query)
        {
            var users = await _userRepo.GetAllUsersToInviteAsync(currentUserId, query);
            return users.Select(u => new UserDTO
            {
                UserID = u.UserID,
                Username = u.Username,
                AvatarUrl = u.AvatarUrl
            }).ToList();
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
             expires: DateTime.Now.AddDays(1),
             signingCredentials: creds
         );
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

        }
    }
}



