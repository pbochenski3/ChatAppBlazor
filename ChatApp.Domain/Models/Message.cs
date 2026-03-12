using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Domain.Models
{
    public class Message
    {
        public Guid MessageID { get; set; } = Guid.CreateVersion7();
        public string Content { get; set; } = null!;
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public Guid ChatID { get; set; }
        public  Chat Chat { get; set; } = null!;
        public Guid SenderID { get; set; }
        public User Sender { get; set; } = null!;
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }

    }
}
