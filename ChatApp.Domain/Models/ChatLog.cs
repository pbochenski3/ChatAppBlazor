using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Domain.Models
{
    public class ChatLog
    {
        public int ChatID { get; set; }
        public string Title { get; set; }
        public ICollection<Message> Messages { get; set; } 
        public ICollection<User> Users { get; set; }
    }
}
