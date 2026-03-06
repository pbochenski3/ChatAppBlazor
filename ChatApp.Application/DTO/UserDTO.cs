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
        public string avatarUrl { get; set; }
    }
}
