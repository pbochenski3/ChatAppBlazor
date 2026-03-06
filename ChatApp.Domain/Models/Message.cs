using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Domain.Models
{
    public class Message
    {
        public Guid MessageID { get; set; } = Guid.CreateVersion7();
        public string Content { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public Guid ChatID { get; set; }
        public  Chat Chat { get; set; }
        public Guid SenderID { get; set; }
        public User Sender { get; set; }

    }
}
