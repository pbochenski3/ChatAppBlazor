using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Domain.Models
{
    public class UserChat
    {
        public Guid UserID { get; set; }
        public User User { get; set; }
        public Guid ChatID { get; set; }
        public Chat Chat { get; set; }
        public string ChatName { get; set; } 
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
        public bool IsAdmin { get; set; } = false;
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
