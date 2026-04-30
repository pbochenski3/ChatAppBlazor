using ChatApp.Domain.Enums;

namespace ChatApp.Domain.Models
{
    public class Message
    {
        public Guid MessageID { get; set; }
        public string Content { get; set; } = null!;
        public string imageUrl { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
        public Guid ChatID { get; set; }
        public Chat Chat { get; set; } = null!;
        public Guid? SenderID { get; set; } = null!;
        public User Sender { get; set; } = null!;
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public MessageType MessageType { get; set; }

        public static Message CreateSystemMessage(Guid chatId, string content)
        {
            return  new Message
            {
                MessageID = Guid.CreateVersion7(),
                ChatID = chatId,
                Content = content,
                MessageType = MessageType.System,
                SentAt = DateTime.UtcNow
            };
        }
    }
}
