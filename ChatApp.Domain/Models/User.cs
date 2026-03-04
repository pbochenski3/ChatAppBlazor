using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Domain.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string Username { get; set; }

        public ICollection<ChatLog> ChatLogs { get; set; }


    }
}
