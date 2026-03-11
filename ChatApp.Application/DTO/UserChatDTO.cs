using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.DTO
{
    internal class UserChatDTO
    {
        public Guid UserID { get; set; }
        public string Username { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public Guid ChatID { get; set; }
        public string ChatName { get; set; } = string.Empty;
        public DateTime JoinedAt { get; set; }
        public bool IsAdmin { get; set; }
    }
}
