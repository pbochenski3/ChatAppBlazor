using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Models
{
    public class Message
    {
        public int MessageID { get; set; }
        public int ChatID { get; set; }
        public ChatLog ChatLog { get; set; }
        public int SenderID { get; set; }
        public User Sender { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
    }
}
