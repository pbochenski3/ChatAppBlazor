using ChatApp.Domain.Enums;

namespace ChatApp.Domain.Models
{
    public class NotificationMessage
    {
        public string Id { get; } = Guid.NewGuid().ToString();
        public string Message { get; set; } = string.Empty;
        public NotificationType Type { get; set; }
        public DateTime Timestamp { get; } = DateTime.UtcNow;
    }
}
