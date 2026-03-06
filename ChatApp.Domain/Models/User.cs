using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Domain.Models
{
    public class User
    {
        public Guid UserID { get; set; } = Guid.CreateVersion7();
        public string Username { get; set; }
        public bool isOnline { get; set; } = false;
        public string avatarUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Password { get; set; }

        public ICollection<UserChat> UserChats { get; set; } = new HashSet<UserChat>();

        public ICollection<Message> Messages { get; set; } = new HashSet<Message>();


    }
}
