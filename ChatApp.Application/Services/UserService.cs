using ChatApp.Application.DTO;
using ChatApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using ChatApp.Application.Interfaces.Repository;
using ChatApp.Application.Interfaces.Service;

namespace ChatApp.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;
        public UserService(IUserRepository userRepo)
        {
            _userRepo = userRepo;
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
            string username = dto.Username;
            var user = await _userRepo.GetByUsernameAsync(username);
           if(user is not null && BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
            {
                await _userRepo.SetStatus(user.UserID,true);
                return new UserDTO() {Username = user.Username,UserID = user.UserID, IsOnline = true};
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

        }
    }



