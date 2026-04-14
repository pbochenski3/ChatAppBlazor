using ChatApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Domain.Models
{
    public class Invite
    {
        public Guid InviteID { get; set; } = Guid.CreateVersion7();

        public Guid SenderID { get; set; }
        public User Sender { get; set; } = null!;

        public Guid ReceiverID { get; set; }
        public User Receiver { get; set; } = null!;

        public InviteStatus Status { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
