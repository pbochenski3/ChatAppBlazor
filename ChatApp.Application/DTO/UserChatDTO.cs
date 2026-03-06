using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.DTO
{
    internal class UserChatDTO
    {
        public Guid UserID { get; set; }
        public string Username { get; set; } // Dodajemy dla wygody frontendu
        public string AvatarUrl { get; set; }

        public Guid ChatID { get; set; }
        public string ChatName { get; set; } // Jeśli czat ma nazwę (np. grupa)

        public DateTime JoinedAt { get; set; }
        public bool IsAdmin { get; set; }
    }
}
