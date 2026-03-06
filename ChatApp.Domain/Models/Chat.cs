using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Domain.Models
{
    public class Chat
    {
        public Guid ChatID { get; set; } = Guid.CreateVersion7();
        public string ChatName { get; set; }
        public ICollection<UserChat> UserChats { get; set; } = new HashSet<UserChat>();
        public ICollection<Message> Messages { get; set; } = new HashSet<Message>();
    }
}
