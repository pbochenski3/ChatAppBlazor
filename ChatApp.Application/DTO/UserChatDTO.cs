using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.DTO
{
    internal class UserChatDTO
    {
        public Guid UserID { get; set; }
        public string Username { get; set; }
        public string AvatarUrl { get; set; }
        public Guid ChatID { get; set; }
        public string ChatName { get; set; }
        public DateTime JoinedAt { get; set; }
        public bool IsAdmin { get; set; }
    }
}
