using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.DTO
{
    public class ContactDTO
    {
        public Guid ContactUserID { get; set; }
        public string Username { get; set; }
        public string AvatarUrl { get; set; }
        public bool IsOnline { get; set; }
        public DateTime AddedAt { get; set; }

        // Pomocnicze: ID czatu 1:1 z tą osobą, jeśli już istnieje
        public Guid? ChatID { get; set; }
    }
}
