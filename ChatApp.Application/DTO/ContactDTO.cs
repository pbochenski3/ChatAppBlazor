using ChatApp.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.DTO
{
    public class ContactDTO : ISidebarItem
    {
        public Guid ContactUserID { get; set; }
        public string Username { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public bool IsOnline { get; set; }
        public DateTime? AddedAt { get; set; }
        public Guid? ChatID { get; set; }

        public Guid Id => ContactUserID;
        public string DisplayName => Username;
        public EnumType ItemType => EnumType.Contact;
    }
}
