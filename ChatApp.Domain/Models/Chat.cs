using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ChatApp.Domain.Models
{
    public class Chat
    {
        public Guid ChatID { get; set; } = Guid.CreateVersion7();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string ChatName { get; set; } = string.Empty;
        public ICollection<UserChat> UserChats { get; set; } = new HashSet<UserChat>();
        public ICollection<Message> Messages { get; set; } = new HashSet<Message>();
        public bool IsDeleted { get; set; } = false;
        public string AvatarUrl { get; set; } = string.Empty;
        public bool IsGroup { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
    }
}
