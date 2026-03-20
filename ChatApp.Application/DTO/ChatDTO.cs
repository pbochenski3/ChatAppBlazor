using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.DTO
{
        public class ChatDTO 
        {
            public Guid ChatID { get; set; }
            public string? Avatar { get; set; }
            public DateTime CreatedAt { get; set; }
            public string ChatName { get; set; } = string.Empty;
            public string AvatarUrl { get; set; } = string.Empty;
            public IEnumerable<UserDTO> Participants { get; set; } = new HashSet<UserDTO>();
            public string LastMessageContent { get; set; } = string.Empty;
            public DateTime? LastMessageAt { get; set; }
            public bool IsDeleted { get; set; } = false;
            public bool IsGroup { get; set; }
            public bool IsArchive { get; set; } = false;
    }
}
