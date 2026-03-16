using ChatApp.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.DTO
{
        public class ChatDTO : ISidebarItem
        {
            public Guid ChatID { get; set; }
            public string? GroupAvatar { get; set; }
            public DateTime CreatedAt { get; set; }
            public string ChatName { get; set; } = string.Empty;
            public IEnumerable<UserDTO> Participants { get; set; } = new HashSet<UserDTO>();
            public string LastMessageContent { get; set; } = string.Empty;
            public DateTime? LastMessageAt { get; set; }
            public bool IsArchive { get; set; } = false;
            public bool IsDeleted { get; set; } = false;
            public bool IsGroup { get; set; }
              
            public Guid Id => ChatID;
            public string DisplayName => ChatName;
        public EnumType ItemType => EnumType.Chat;
            public string? AvatarUrl => GroupAvatar;
            public bool IsOnline => true;

    }
}
