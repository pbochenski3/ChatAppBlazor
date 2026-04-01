using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.DTO.Chats
{
    public class LastMessageDTO
    {
        public Guid? LastMessageID { get; set; }
        public string? LastMessageContent { get; set; } = string.Empty;
        public string? LastMessageSender { get; set; } = string.Empty;
        public DateTime LastMessageAt { get; set; }
    }
}
