using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Domain.Models
{
    public class User
    {
        public Guid UserID { get; set; } = Guid.CreateVersion7();
        public string Username { get; set; } = string.Empty;
        public bool IsOnline { get; set; } = false;
        public string AvatarUrl { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Password { get; set; } = string.Empty;

        public ICollection<UserChat> UserChats { get; set; } = new HashSet<UserChat>();
        public ICollection<Contact> Contacts { get; set; } = new HashSet<Contact>();
        public ICollection<Invite> SentInvites { get; set; } = new HashSet<Invite>();
        public ICollection<Invite> ReceivedInvites { get; set; } = new HashSet<Invite>();
        public ICollection<Message> Messages { get; set; } = new HashSet<Message>();
    }
}
