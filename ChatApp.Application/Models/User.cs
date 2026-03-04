using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string Username { get; set; }

        public string Password { get; set; }
        public ICollection<ChatLog> ChatLogs { get; set; }


    }
}
