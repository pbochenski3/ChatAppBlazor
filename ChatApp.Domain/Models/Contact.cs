using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Domain.Models
{
    public class Contact
    {
        public Guid UserID { get; set; }
        public User User { get; set; } = null!;
        public Guid ContactUserID { get; set; }
        public User ContactUser { get; set; } = null!;
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;
        public DateTime DeletedAt { get; set ;  }
    }
}
