using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.DTO.Chats
{
    public class ChatIdentityDTO
    {
        public Guid UserID { get; set; }
        public string Username { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public Guid ChatID { get; set; }
        public string ChatName { get; set; } = string.Empty;
        public bool IsGroup { get; set; } = false;
        public Guid? OtherUserId { get; set; }
    }
}
