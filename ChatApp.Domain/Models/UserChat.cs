using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Domain.Models
{
    public class UserChat
    {
        public Guid UserID { get; set; }
        public User User { get; set; } = null!;

        public Guid ChatID { get; set; }
        public Chat Chat { get; set; } = null!;

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
        public bool IsAdmin { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public bool IsArchive { get; set; } = false;
    }
}
