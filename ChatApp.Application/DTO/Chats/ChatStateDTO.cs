using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.DTO.Chats
{
    public class ChatStateDTO
    {
        public bool IsAdmin { get; set; }
        public bool IsArchive { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
        public Guid? LastReadMessageID { get; set; }
        public DateTime LastReadAt { get; set; }
        public int UnreadMessageCount { get; set; }
    }
}
