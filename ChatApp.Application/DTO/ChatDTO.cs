using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.DTO
{
        public class ChatDTO
        {
            public Guid ChatID { get; set; }
            public DateTime CreatedAt { get; set; }
            public string ChatName { get; set; }
            public List<UserDTO> Participants { get; set; } = new();
            public string LastMessageContent { get; set; }
            public DateTime? LastMessageAt { get; set; }
        }
}
