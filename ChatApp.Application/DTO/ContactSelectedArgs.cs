using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.DTO
{
    public class ContactSelectedArgs
    {
        public Guid ChatId { get; set; }
        public bool Force { get; set; } = false;
        public ContactSelectedArgs(Guid chatId, bool force = false)
        {
            ChatId = chatId;
            Force = force;
        }
    }
}
