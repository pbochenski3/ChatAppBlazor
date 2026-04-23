using ChatApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.DTO
{
    public class UserDTO
    {
        public Guid UserID { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool IsOnline { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Token { get; set; }
        public static UserDTO CreateUserDto(User user,string token)
        {
            return new UserDTO
            {
                Username = user.Username,
                UserID = user.UserID,
                IsOnline = true,
                Token = token,
                AvatarUrl = user.AvatarUrl
            };
        }
    }
}
