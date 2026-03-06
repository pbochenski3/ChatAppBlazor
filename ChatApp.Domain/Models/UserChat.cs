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

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
        public bool IsAdmin { get; set; } = false;
    }
}
