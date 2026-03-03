using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.Models
{
    public class ChatLog
    {
        public DateTime MessageSendTime { get; set; }
        public string Message { get; set; }
    }
}
