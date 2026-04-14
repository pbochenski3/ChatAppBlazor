using ChatApp.Domain.Enums;
using ChatApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.DTO
{
    public class InviteDTO
    {
        public Guid InviteID { get; set; } = Guid.CreateVersion7();
        public Guid ReceiverID { get; set; }
        public Guid SenderID { get; set; }
        public string SenderUsername { get; set; } = string.Empty;
        public string ReceiverUsername { get; set; } = string.Empty;
        public string SenderAvatarUrl { get; set; } = string.Empty;
        public InviteStatus Status { get; set; } 
    }
}
