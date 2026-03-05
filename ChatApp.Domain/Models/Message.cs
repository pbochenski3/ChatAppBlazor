using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Domain.Models
{
    public class Message
    {
        public int MessageID { get; set; }
        public int ChatID { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
        public int SenderID { get; set; }
        public User Sender { get; set; }

    }
}
