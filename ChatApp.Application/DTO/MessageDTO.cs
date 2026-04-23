using ChatApp.Domain.Enums;

namespace ChatApp.Application.DTO
{
    public class MessageDTO
    {
        public Guid MessageID { get; set; }
        public string Content { get; set; } = string.Empty;
        public string imageUrl { get; set; } = string.Empty;
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public Guid? SenderID { get; set; }
        public Guid ReceiverId { get; set; }
        public string SenderUsername { get; set; } = string.Empty;
        public Guid ChatID { get; set; }
        public MessageType MessageType { get; set; }
    }
}
