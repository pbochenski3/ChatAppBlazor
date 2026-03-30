using System;

namespace ChatApp.Application.DTO
{
    public class UserChatDTO
    {
        public Guid UserID { get; set; }
        public string AvatarUrl { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public Guid ChatID { get; set; }
        public string ChatName { get; set; } = string.Empty;
        public DateTime JoinedAt { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsArchive { get; set; } = false;
        public DateTime? ArchivedAt { get; set; }
        public bool IsGroup { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public Guid? LastReadMessageID { get; set; }
        public DateTime LastReadAt { get; set; }
        public Guid? LastMessageID { get; set; }
        public DateTime LastMessageAt { get; set; }
        public int UnreadMessageCount { get; set; }
    }
}
