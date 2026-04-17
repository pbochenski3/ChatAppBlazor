using ChatApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Domain.Models
{
    public class NotificationMessage
    {
        public string Id { get; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public NotificationType Type { get; set; }
        public DateTime Timestamp { get; } = DateTime.UtcNow;
    }
}
