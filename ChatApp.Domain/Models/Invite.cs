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
        public async static Task<Contact[]> CreateContact(Guid userId1,Guid userId2)
        {
            var contact1 = new Contact
            {
                UserID = userId1,
                ContactUserID = userId2,
            };
            var contact2 = new Contact
            {
                UserID = userId1,
                ContactUserID = userId2,
            };
            var contacts = new Contact[] { contact1, contact2 };
            return contacts;
        }
    }
}
