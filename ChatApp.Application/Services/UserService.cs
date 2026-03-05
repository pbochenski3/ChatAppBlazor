using ChatApp.Application.DTO;
using ChatApp.Domain.Models;
using ChatApp.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

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
                    Password = BCrypt.Net.BCrypt.HashPassword(dto.Password)
                };
                await _userRepo.RegisterAsync(user);
                await _userRepo.SaveChangesAsync();
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
                return new UserDTO() {Username = user.Username,UserID = user.UserID};
            }
            else
            {
                throw new Exception("Invalid username or password.");
            }
        }

    }
}
