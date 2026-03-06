using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.DTO
{
    public class InviteDTO
    {
        public Guid InviteID { get; set; } = Guid.CreateVersion7();
        public Guid OtherUserID { get; set; }
        public string OtherUsername { get; set; }
        public string OtherAvatarUrl { get; set; }

        public string Status { get; set; } 
        public DateTime CreatedAt { get; set; }

        public bool IsOutgoing { get; set; }
    }
}
