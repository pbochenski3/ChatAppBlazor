using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.DTO
{
        public class ChatDTO
        {
            public Guid ChatID { get; set; }
            public DateTime CreatedAt { get; set; }
            public string ChatName { get; set; } = string.Empty;
            public IEnumerable<UserDTO> Participants { get; set; } = new HashSet<UserDTO>();
            public string LastMessageContent { get; set; } = string.Empty;
            public DateTime? LastMessageAt { get; set; }
        }
}
