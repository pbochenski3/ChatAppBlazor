using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.DTO
{
    internal class UserChatDTO
    {
        public Guid UserID { get; set; }
        public string Username { get; set; } = string.Empty;
        public Guid ChatID { get; set; }
        public string ChatName { get; set; } = string.Empty;
        public DateTime JoinedAt { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsArchive { get; set; } = false;
        public DateTime? ArchivedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public Guid? LastReadMessageID { get; set; }
        public DateTime LastReadAt { get; set; }
        public Guid? LastMessageID { get; private set; }
        public DateTime LastMessageAt { get; set; }
    }
}
