using ChatApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Application.DTO
{
    public class MessageDTO
    {
        public Guid MessageID { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
        public Guid SenderID { get; set; }
        public Guid ReceiverId { get; set; }
        public string SenderUsername { get; set; } = string.Empty;
        public Guid ChatID { get; set; }
    }
}
